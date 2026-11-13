
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using terraria.Blocks;
using terraria.Bosses;
using terraria.Players;
using terraria.Collision;
using terraria.Entity;
using static terraria.Constants;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;



namespace terraria.Rooms;

public class RoomManager
{
    private readonly RoomFactory _roomFactory;
    private readonly List<IBlock> _blocks;
    private List<SpecialBlock> _specialBlocks;
    private readonly BossManager _bossManager;
    private readonly IPlayer _player;

    public List<CollisionSurface> WorldMesh {get; set;} //contains all merged collision surfaces (this is to prevent internal collisions behaving weirdly)

 
    private readonly string[] _roomFiles =
    {
        "SlimeRoom.csv",
        "BeeRoom.csv",
        "CthulhuRoom.csv"
    };

    private int _currRoom;

    public RoomManager(RoomFactory roomFactory, BossManager bossManager, IPlayer player)
    {
        _roomFactory = roomFactory;
        _blocks = new List<IBlock>();
        _specialBlocks = _blocks.OfType<SpecialBlock>().ToList<SpecialBlock>();
        _bossManager = bossManager;
        _player = player;
        _currRoom = 0;
        WorldMesh = new List<CollisionSurface>();
    }

      public void LoadMesh(List<IBlock> blockList)
    {
        ArgumentNullException.ThrowIfNull(blockList);

        WorldMesh.Clear();//make sure previous room's mesh doesent persist
        
        //very efficient for lookups
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        foreach(IBlock b in blockList)
        {
            occupiedPositions.Add(b.Position);
        }


        HashSet<Vector2> mergedHorizontally = new HashSet<Vector2>();//tracks blocks already merged horizontally so vertical pass doesent need to scan them
        

        //Horizontal pass       
        for (int i = 0; i < blockList.Count; i++)
        {
            Vector2 startPos = blockList[i].Position;

            bool hasBlockOnTop = occupiedPositions.Contains(new Vector2(startPos.X, startPos.Y - BlockSize));
            if (mergedHorizontally.Contains(startPos) || hasBlockOnTop)
            {
                continue;
            }

            int runLength = 1;

            bool nextExists = true;
            bool nextHasBlockOnTop = false;
            while (nextExists && !nextHasBlockOnTop)
            {
                Vector2 nextPos = new Vector2(startPos.X + (runLength * BlockSize), startPos.Y);
                nextExists = occupiedPositions.Contains(nextPos);
                nextHasBlockOnTop = occupiedPositions.Contains(new Vector2(nextPos.X, nextPos.Y - BlockSize));

                if (nextExists && !nextHasBlockOnTop)
                {
                    runLength++;
                }
            }

            
            bool hasBlockBelow = occupiedPositions.Contains(new Vector2(startPos.X, startPos.Y + BlockSize));
            //means this is likely the top of a column
            if (runLength == 1 && hasBlockBelow)
            {
                continue;
            }

            // all blocks in this strip have been merged
            for (int k = 0; k < runLength; k++)
            {
                mergedHorizontally.Add(new Vector2(startPos.X + (k * BlockSize), startPos.Y));
            }

            WorldMesh.Add(new CollisionSurface(new Vector2((int)startPos.X,(int)startPos.Y), runLength * BlockSize, false));
        }



        //remaining vertical pass
        int j = 0;
        while (j < blockList.Count)
        {
            Vector2 startPos = blockList[j].Position;

            // sip if already scanned by the horizontal pass
            if (mergedHorizontally.Contains(startPos))
            {
                j++;
                continue;
            }

            bool hasExposedSide = !occupiedPositions.Contains(new Vector2(startPos.X - BlockSize, startPos.Y)) || !occupiedPositions.Contains(new Vector2(startPos.X + BlockSize, startPos.Y));//check both sides
            if (!hasExposedSide)
            {
                j++;
                continue;
            }

            int runHeight = 1;
            while (j + runHeight < blockList.Count)
            {
                Vector2 nextPos = blockList[j + runHeight].Position;
                Vector2 expectedPos = new Vector2(startPos.X, startPos.Y + (runHeight * BlockSize));

                bool nextHasExposedSide = !occupiedPositions.Contains(new Vector2(nextPos.X - BlockSize, nextPos.Y)) || !occupiedPositions.Contains(new Vector2(nextPos.X + BlockSize, nextPos.Y));

                if (nextPos == expectedPos && !mergedHorizontally.Contains(nextPos) && nextHasExposedSide)
                {
                    runHeight++;
                }
                else
                {
                    break;
                }
            }


            WorldMesh.Add(new CollisionSurface(new Vector2((int)startPos.X,(int)startPos.Y), runHeight * BlockSize, true));

            j += runHeight;
        }


    }

    public event Action RoomLoaded;

    public void LoadCurrentRoom()
    {
        RoomLoaded?.Invoke();
        _blocks.Clear();
        _specialBlocks.Clear();
        _bossManager.Clear();

        string roomFile = Path.Combine(
            AppContext.BaseDirectory,
            "Content",
            "Rooms",
            _roomFiles[_currRoom]
        );

        List<RoomObjectData> roomData = _roomFactory.LoadRoom(roomFile);

        for (int i = 0; i < roomData.Count; i++)
        {
            object gameObject = _roomFactory.CreateObject(roomData[i]);

            if (gameObject is IBlock block)
            {
                _blocks.Add(block);
            }
            else if (gameObject is IBoss boss)
            {
                _bossManager.Add(boss);
            }
        }

        LoadMesh(_blocks);//load collision mesh

        _specialBlocks = _blocks.OfType<SpecialBlock>().ToList<SpecialBlock>();

        _player.Position = new Vector2(200, 450);
        _player.Velocity = Vector2.Zero;
    }

    public void ResetRoom()
    {
        _currRoom = 0;
        LoadCurrentRoom();
    }

    public void NextRoom()
    {
        if (_currRoom < _roomFiles.Length - 1)
        {
            _currRoom++;
            LoadCurrentRoom();
        }
    }

    public void PreviosRoom()
    {
        if (_currRoom > 0)
        {
            _currRoom--;
            LoadCurrentRoom();
        }
    }
    public void ProcessWorldCollisions(IEntity e)
    {
        ArgumentNullException.ThrowIfNull(e);
        float movementMultiplier = e is IBoss boss ? boss.Health.Effects.MovementMultiplier : 1f;
        //process x collision first
        Vector2 loc = new Vector2(e.Position.X, e.Position.Y);
        loc.X += e.Velocity.X * movementMultiplier; // project one tick into the future
        e.Position = loc;
        e.Hitbox.Update(e.Position);

        for (int i = 0; i < WorldMesh.Count; i++)
        {
            CollisionSurface surface = WorldMesh[i];
            if (!surface.VerticalOrientation) continue;

            Rectangle surfaceRect = surface.BoundingBox;
            if (e.Hitbox.BoundingBox.Intersects(surfaceRect))
            {
                ResolveXAxisCollision(e,surface, surfaceRect);
                e.Hitbox.Update(e.Position);
            }
        }

        

        e.IsGrounded = false;

        // process y collision second
        loc = new Vector2(e.Position.X, e.Position.Y);
        loc.Y += e.Velocity.Y * movementMultiplier; // project one tick into the future
        e.Position = loc;
        e.Hitbox.Update(e.Position);

        for (int i = 0; i < WorldMesh.Count; i++)
        {
            CollisionSurface surface = WorldMesh[i];
            if (surface.VerticalOrientation) continue;

            Rectangle surfaceRect = surface.BoundingBox;
            if (e.Hitbox.BoundingBox.Intersects(surfaceRect))
            {
                ResolveYAxisCollision(e,surface, surfaceRect);
                e.Hitbox.Update(e.Position);
            }
        }
        UpdatePlayerGroundContact();
    }

    private void UpdatePlayerGroundContact()
    {
        // Rectangle bounds truncate subpixel movement. Keep floor contact when
        // gravity moves the feet less than one pixel, but never during a jump.
        if (_player.Velocity.Y < 0 || _player.IsGrounded) return;

        float feet = _player.Position.Y + _player.Hitbox.BoundingBox.Height;
        Rectangle bounds = _player.Hitbox.BoundingBox;
        foreach (CollisionSurface surface in WorldMesh)
        {
            if (surface.VerticalOrientation) continue;
            Rectangle floor = surface.BoundingBox;
            float gap = floor.Top - feet;
            if (bounds.Right > floor.Left && bounds.Left < floor.Right &&
                gap >= -1f && gap <= 1f)
            {
                _player.Position = new Vector2(_player.Position.X,
                    floor.Top - bounds.Height);
                _player.Velocity = new Vector2(_player.Velocity.X, 0f);
                _player.IsGrounded = true;
                _player.Hitbox.Update(_player.Position);
                return;
            }
        }
    }

    private void ResolveXAxisCollision(IEntity e, CollisionSurface surface, Rectangle surfaceRect)
    {
        Rectangle bounds = e.Hitbox.BoundingBox;
        Vector2 newLocation = e.Position;

        if (e is IPlayer player)
        {
            if (e.Velocity.X > 0)
            {
                //moving right, push left
                newLocation.X = surfaceRect.Left - bounds.Width;
            }
            else if (e.Velocity.X < 0)
            {
                //moving left, push right
                newLocation.X = surfaceRect.Right;
            
        }
        // Check if the entity is a boss
        else if (e is IBoss boss)
        {
            float halfWidth = bounds.Width / 2f;

            if (e.Velocity.X > 0)
            {
                //moving right, push left
                newLocation.X = surfaceRect.Left - halfWidth;
            }
            else if (e.Velocity.X < 0)
            {
                //moving left, push right
                newLocation.X = surfaceRect.Right + halfWidth;
            }
        }

        
        }

        e.Position = newLocation;
        e.Velocity = new Vector2(0, e.Velocity.Y);
    }

    private void ResolveYAxisCollision(IEntity e, CollisionSurface surface, Rectangle surfaceRect)
    {
        Rectangle bounds = e.Hitbox.BoundingBox;
        Vector2 newLocation = e.Position;

        if (e is IPlayer player)
        {
            if (e.Velocity.Y > 0)
            {
                //falling, push poiu
                newLocation.Y = surfaceRect.Top - bounds.Height;
                e.IsGrounded = true;
            }
            else if (e.Velocity.Y < 0)
            {
                //moving up, puish down
                newLocation.Y = surfaceRect.Bottom;
            }
            else
            {
                e.IsGrounded = false;
            }
        }
        // Check if the entity is a boss
        else if (e is IBoss boss)
        {
            float halfHeight = bounds.Height / 2f;
            if (e.Velocity.Y > 0)
            {
                //falling, push poiu
                newLocation.Y = surfaceRect.Top - halfHeight;
                e.IsGrounded = true;
            }
            else if (e.Velocity.Y < 0)
            {
                //moving up, puish down
                newLocation.Y = surfaceRect.Bottom + halfHeight;
            }
        }

        

        e.Position = newLocation;
        e.Velocity = new Vector2(e.Velocity.X, 0);
    }

    private void DEBUGHitbox(SpriteBatch sb)
    {
        foreach(CollisionSurface s in WorldMesh)
        {
            s.DrawDebug(sb);
        }
    }

    public void DrawBlocks(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocks[i].Draw(spriteBatch);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawBlocks(spriteBatch);

        if (DEBUG.ShowHitboxes)
        {
            DEBUGHitbox(spriteBatch);
        }
    }

    public void UpdateBlocks(GameTime gameTime)
    {
        for (int i = 0; i < _blocks.Count; i++)
        {
        _blocks[i].Update(gameTime);
        }
    }


    public void SpecialBlockCollision()
    {
        // Use the resolved position, including a one-pixel probe below the feet.
        _player.SpecialHitbox.Update(_player.Position);
        _player.MovementEffects = PlayerMovementEffects.None;
        foreach(SpecialBlock sb in _specialBlocks)
        {
            if (_player.SpecialHitbox.Intersects(sb.Hitbox))
            {
                if (_player.MovementEffects != PlayerMovementEffects.Burning)
                {
                    _player.MovementEffects = sb.MovementEffect;
                }
            }
        }
        
    }

    public void Update(IPlayerInput input, GameTime gameTime)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.IsNewPress(KeyAction.Reset))
        {
            ResetRoom();
        }
        else if (input.IsNewPress(KeyAction.NextRoom))
        {
                NextRoom();
        }
        else if (input.IsNewPress(KeyAction.PreviousRoom))
        {
            PreviosRoom();
        }
        
        UpdateBlocks(gameTime);

        ProcessWorldCollisions(_player);
        bool bossAlive = false;
        foreach(IBoss b in _bossManager.Bosses)
        {
            if(b.Health.CurrentHealth > 0)
            {
                bossAlive = true;
            }
            ProcessWorldCollisions(b);
        }

        if (!bossAlive)
        {
            NextRoom();
        }


        SpecialBlockCollision();
    }
}
