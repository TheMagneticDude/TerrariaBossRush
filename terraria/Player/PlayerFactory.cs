using Microsoft.Xna.Framework.Content;
using MonoGameLibrary.Graphics;
using terraria.Hotbar;
using terraria.Items;
using terraria.Projectiles;
using terraria.Weapons;

namespace terraria.Players;

internal static class PlayerFactory
{
    public static Player Create(ContentManager content, PlayerInput input, ProjectileManager projectiles)
    {
        TextureAtlas playerAtlas = TextureAtlas.FromFile(content, "Player/Player.xml");

        IHotbarEntry[] hotbarEntries =
        [
            new WeaponHotbarEntry("Wooden Sword", WeaponFactory.Instance.CreateSword(), false),
            new WeaponHotbarEntry("Magic Dagger", WeaponFactory.Instance.CreateDagger(projectiles), false),
            new WeaponHotbarEntry("Bow", WeaponFactory.Instance.CreateBow(projectiles), false),
            ItemFactory.Instance.CreateHealthPot(),
            ItemFactory.Instance.CreateRecallPot(),
            new WeaponHotbarEntry("Staff", WeaponFactory.Instance.CreateStaff(projectiles), false),
            new WeaponHotbarEntry("Yo-yo", WeaponFactory.Instance.CreateYoyo(), false),
            new WeaponHotbarEntry("Whip", WeaponFactory.Instance.CreateWhip(), false),
            new WeaponHotbarEntry("Fire Bow", WeaponFactory.Instance.CreateFireBow(projectiles), false),
            new WeaponHotbarEntry("Frost Staff", WeaponFactory.Instance.CreateFrostStaff(projectiles), false)
        ];

        PlayerHotbar hotbar = new PlayerHotbar(hotbarEntries);
        return new Player(playerAtlas, hotbar, input);
    }

}
