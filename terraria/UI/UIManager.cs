using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

using terraria.Players;

namespace terraria.UI;

public class UIManager
{
    private HotbarUI _hotbarUI;
    private HealthBar _healthBar;

    private ManaBar _manaBar;
    private BuffIcon _swiftnessIcon;
    private Texture2D _blankTexture;

    public UIManager(ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);

        _hotbarUI = new HotbarUI();

        Texture2D heart = content.Load<Texture2D>("UI/heart");
        Texture2D emptyHeart = content.Load<Texture2D>("UI/empty_heart");
        Texture2D star = content.Load<Texture2D>("UI/star");
        Texture2D swiftnessIcon = content.Load<Texture2D>("UI/swiftnessIcon");

        _healthBar = new HealthBar(heart, emptyHeart, new Vector2(925, 20));
        _manaBar = new ManaBar(star, new Vector2(925, 54));
        _swiftnessIcon = new BuffIcon(swiftnessIcon, new Vector2(1238, 100)
);
    }

    public void Draw(SpriteBatch spriteBatch, IPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(spriteBatch);

        if (_blankTexture == null)
        {
            _blankTexture = new Texture2D(
                spriteBatch.GraphicsDevice, 1, 1);

            _blankTexture.SetData(new[] { Color.White });
        }


        _hotbarUI.Draw(spriteBatch, player.Hotbar);

        _healthBar.Draw(spriteBatch, player.CurrentHealth, player.MaxHealth);
        _manaBar.Draw(spriteBatch, player.CurrentMana, player.MaxMana);

        if (player.HasSwiftness)
        {
            _swiftnessIcon.Draw(spriteBatch);
        }

        if (player.IsDead)
        {
            DeathOverlay.Draw(spriteBatch, _blankTexture);
        }
    }

}