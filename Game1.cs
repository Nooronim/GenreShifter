using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using GenreShifterProt4.Sprites;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace GenreShifterProt4
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Vector2 baseScreenSize = new Vector2(800, 480);
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;

        private Sprite[] playerSprite;
        private Sprite[] platforms;
        private List<Enemy> enemies;
        private List<Sword> sword;
        private List<LazerBeam> redBeams;
        private List<LazerBeam> greenBeams;


        SpriteFont font;
        Texture2D scoreBarSprite;
        SpriteFont smallFont;

        public int Score = 0;

        Random rnd = new Random();
        int genreNum;
        int nextGenre;

        TimeSpan gameTimer = TimeSpan.FromSeconds(0);
        double timeBetweenGames;

        Genre[] allGenres = new Genre[4];

        int[] fakeTime;
        double fakeSeconds;
        int switcher;

        //Player
        bool playerInAir;
        float playerJumpingSpeed;
        //float playerJumpHelper;
        bool[] playerOnPlat;
        bool hitJump;

        string playerLastDirection;
        //float playerLastDirectionNum; // 1 = right, 2 = left, 3 = up, 4 = down
        private Vector2 swordStartPos;
        private TimeSpan swordTimer = TimeSpan.FromSeconds(0);
        public Texture2D[] swordTextures;
        public int swordTextureNum;

        private Vector2 beamStartPos;
        private Vector2 greenBeamStartPos;
        public Texture2D[] redBeamTextures;
        public Texture2D[] greenBeamTextures;
        public int redBeamTextureNum;
        public int greenBeamTextureNum;
        public int greenBeamDirectionNum; // 1 = right, 2 = left, 3 = up, 4 = down
        public bool canShoot;
        public string beamDirection;
        private TimeSpan shootTimer = TimeSpan.FromSeconds(0);

        private GamePadState gamePadState;
        private TouchCollection touchState;
        private VirtualGamePad virtualGamePad;
        private string whichBtn;
        private bool continuePressed;

        public int playerHP;
        Texture2D[] playerHPTextures;
        private bool wasContinuePressed;
        private TimeSpan invisFramesTimer = TimeSpan.FromSeconds(0);
        private bool isInvis;


        public int numOfEnemies;
        public float enemySpeedChanger;
        public float enemySpeed;

        public Vector2 enemyPos;
        public Rectangle enemyRect
        {
            get
            {
                return new Rectangle((int)enemyPos.X, (int)enemyPos.Y, 150, 150);
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        public void CreateGames()
        {
            allGenres[0] = new Genre("Platformer", Content.Load<Texture2D>("Backgrounds/platformerBG"), Content.Load<Texture2D>("Enemies/platformerEnemy"), Content.Load<Texture2D>("Platforms/platformerPlat"), "Jump", false);
            allGenres[1] = new Genre("Adventure", Content.Load<Texture2D>("Backgrounds/adventureBG"), Content.Load<Texture2D>("Enemies/adventureEnemy"), Content.Load<Texture2D>("Platforms/adventurePlat"), "Sword", true);
            allGenres[2] = new Genre("Space Shooter", Content.Load<Texture2D>("Backgrounds/spaceBG"), Content.Load<Texture2D>("Enemies/spaceEnemy"), Content.Load<Texture2D>("Platforms/spacePlat"), "Shoot", true);
            allGenres[3] = new Genre("gravity", Content.Load<Texture2D>("greenBG"), Content.Load<Texture2D>("greenEnemy"), Content.Load<Texture2D>("greenPlat"), "Change Gravity", false);
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            timeBetweenGames = 15;
            genreNum = 2;
            nextGenre = 1;

            fakeTime = new int[15] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            fakeSeconds = 1;
            switcher = 0;

            //playerJumpHelper = 0;
            playerInAir = false;
            playerJumpingSpeed = 0;
            playerOnPlat = new bool[2] { false, false };
            hitJump = false;

            playerHP = 3;
            isInvis = false;

            numOfEnemies = 1;
            enemySpeedChanger = 0.03f;
            enemySpeed = 0;

            swordStartPos = new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2 + 76), (_graphics.GraphicsDevice.Viewport.Height - 100 - 80));
            swordTextureNum = 0;

            beamStartPos = new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2 + 76), (_graphics.GraphicsDevice.Viewport.Height - 83 - 80));
            greenBeamStartPos = new Vector2((baseScreenSize.X / 2 + 1), baseScreenSize.Y / 2 - 75);
            redBeamTextureNum = 0;
            canShoot = true;
            beamDirection = "left";

            continuePressed = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            baseScreenSize = new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);
            ScalePresentationArea();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            font = Content.Load<SpriteFont>("scoreFont");
            smallFont = Content.Load<SpriteFont>("scoreFontSmall");
            scoreBarSprite = Content.Load<Texture2D>("scoreBar");
            //playerSprite = Content.Load<Texture2D>("Players/player");
            playerHPTextures = new Texture2D[4]
            {
                Content.Load<Texture2D>("HP/HP0"),
                Content.Load<Texture2D>("HP/HP1"),
                Content.Load<Texture2D>("HP/HP2"),
                Content.Load<Texture2D>("HP/HP3")
            };


            virtualGamePad = new VirtualGamePad(baseScreenSize, globalTransformation, Content.Load<Texture2D>("GamePad/GamePadButton"), Content.Load<Texture2D>("GamePad/GamePadConnector"), Content.Load<Texture2D>("GamePad/ActionButton"));

            var playerTexture = Content.Load<Texture2D>("Players/player");

            CreateGames();

            playerSprite = new Sprite[]
            {
                new Player(playerTexture)
                {
                    Position = new Vector2((_graphics.GraphicsDevice.Viewport.Width/2 - 75), (_graphics.GraphicsDevice.Viewport.Height-150-80)),
                    Color = Color.White,
                    Speed = 5f,
                    Input = virtualGamePad,
                    Direction = "right",
                    //Opacity = 100f,
                },
            };

            platforms = new Sprite[]
            {
                new Platform(allGenres[genreNum].platform)
                {
                    Position = new Vector2((_graphics.GraphicsDevice.Viewport.Width/2 - 600), (_graphics.GraphicsDevice.Viewport.Height/2 + 30)),
                    Color = Color.White,
                    Speed = 0f,
                    Input = virtualGamePad,
                    Direction = "right",
                    //Opacity = 100f,
                },

                new Platform(allGenres[genreNum].platform)
                {
                    Position = new Vector2((_graphics.GraphicsDevice.Viewport.Width/2 + 200), (_graphics.GraphicsDevice.Viewport.Height/2 + 30)),
                    Color = Color.White,
                    Speed = 0f,
                    Input = virtualGamePad,
                    Direction = "right",
                    //Opacity = 100f,
                },
            };

            enemies = new List<Enemy>()
            {
                new Enemy(allGenres[genreNum].enemy)
                {
                    Position = new Vector2((baseScreenSize.X / 2 - 75), baseScreenSize.Y / 2 - 75),
                    Color = Color.White,
                    Speed = 0f,
                    Input = virtualGamePad,
                    Direction = "right",
                    attackFrequency = rnd.Next(1,6),
                    //Opacity = 1f,
                },
            };

            sword = new List<Sword>();
            swordTextures = new Texture2D[4]
            {
                Content.Load<Texture2D>("Items/adventureSwordLeft"),
                Content.Load<Texture2D>("Items/adventureSwordRight"),
                Content.Load<Texture2D>("Items/adventureSwordUp"),
                Content.Load<Texture2D>("Items/adventureSwordDown")
            };

            redBeams = new List<LazerBeam>();
            greenBeams = new List<LazerBeam>();

            redBeamTextures = new Texture2D[2]
            {
                Content.Load<Texture2D>("Items/spaceBeamRightLeft"),
                Content.Load<Texture2D>("Items/spaceBeamUpDown")
            };

            greenBeamTextures = new Texture2D[2]
            {
                Content.Load<Texture2D>("Items/spaceEnemyBeamRightLeft"),
                Content.Load<Texture2D>("Items/spaceEnemyBeamUpDown")
            };

        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
               backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
                ScalePresentationArea();

            TimerAndGenreChange(gameTime);

            foreach (var sprite in playerSprite)
                sprite.Update(gameTime, playerSprite);

            foreach (var enemy in enemies)
                enemy.Update(gameTime, enemies.ToArray());

            foreach (var Sword in sword)
                Sword.Update(gameTime, sword.ToArray());

            foreach (var Beam in redBeams)
                Beam.Update(gameTime, redBeams.ToArray());

            foreach (var Beam in greenBeams)
                Beam.Update(gameTime, greenBeams.ToArray());

            HandleInput(gameTime);

            foreach (var platform in platforms)
                platform._texture = allGenres[genreNum].platform;

            foreach (var enemy in enemies)
                enemy._texture = allGenres[genreNum].enemy;

            foreach (var sprite in playerSprite)
            {
                if (sprite.Velocity != Vector2.Zero)
                    virtualGamePad.NotifyPlayerIsMoving();
            }


            //Player Movement
            CollisionWithBorders();
            playerSprite[0].Direction = playerLastDirection;
            CollisionWithPlatform();
            if (!hitJump)
            {
                if (playerOnPlat[0] || playerOnPlat[1])
                    playerInAir = false;
                else
                    playerInAir = true;
            }
            if (playerInAir && (allGenres[genreNum].canUp == false))
                PlayerJump();

            //Enemy Related
            CollisionWithEnemy();
            RegenEnemis();
            ScoreHandler();
            EnemyMovement(gameTime);
            InvisibillityFrames(gameTime);

            //Sword Related
            SwordRemover(gameTime);
            SlashingEnemies();

            //Beamssss wtf
            MoveBeams();
            ShootCooldown(gameTime);
            RedBeamsCollision();

            for (int Beam = greenBeams.Count - 1; Beam >= 0; Beam--)
            {
                if (greenBeams[Beam].Position.X > (baseScreenSize.X + 1) ||
                    greenBeams[Beam].Position.X < (baseScreenSize.X - baseScreenSize.X - 51) ||
                    greenBeams[Beam].Position.Y > (baseScreenSize.Y + 1) ||
                    greenBeams[Beam].Position.Y < (baseScreenSize.Y - baseScreenSize.Y - 51))
                {
                    greenBeams.RemoveAt(Beam);
                }

                else if (greenBeams[Beam].IsTouchingRight(playerSprite[0]) ||
                         greenBeams[Beam].IsTouchingLeft(playerSprite[0]) ||
                         greenBeams[Beam].IsTouchingTop(playerSprite[0]) ||
                         greenBeams[Beam].IsTouchingBottom(playerSprite[0]))
                {
                    playerHP -= 1;
                    //greenBeams.RemoveAt(Beam);
                    isInvis = true;
                    playerSprite[0].Color = Color.Gold;
                    System.Diagnostics.Debug.WriteLine("ouch");
                }
            }


            base.Update(gameTime);
        }

        private void RedBeamsCollision()
        {
            for (int Beam = redBeams.Count - 1; Beam >= 0; Beam--)
            {
                if (redBeams[Beam].Position.X > (baseScreenSize.X + 1) ||
                    redBeams[Beam].Position.X < -51 ||
                    redBeams[Beam].Position.Y > (baseScreenSize.Y + 1) ||
                    redBeams[Beam].Position.Y < -51)
                {
                    redBeams.RemoveAt(Beam);
                }

                else
                {
                    //System.Diagnostics.Debug.WriteLine("num of beams is {0}", redBeams.Count);
                    for (int enemy = enemies.Count - 1; enemy >= 0; enemy--)
                    {
                        if (redBeams[Beam].IsTouchingRight(enemies[enemy]) ||
                            redBeams[Beam].IsTouchingLeft(enemies[enemy]) ||
                            redBeams[Beam].IsTouchingTop(enemies[enemy]) ||
                            redBeams[Beam].IsTouchingBottom(enemies[enemy]))
                        {
                            enemies.RemoveAt(enemy);
                            isInvis = true;
                            playerSprite[0].Color = Color.Gold;
                            Score += 1;
                            //redBeams.RemoveAt(Beam);
                        }
                    }
                }
            }
        }

        private void ShootCooldown(GameTime gameTime)
        {
            if (!canShoot)
            {
                shootTimer += gameTime.ElapsedGameTime;
                if (shootTimer.TotalSeconds >= 0.3)
                {
                    canShoot = true;
                    shootTimer = TimeSpan.FromSeconds(0);
                }
            }
        }

        private void MoveBeams()
        {
            foreach (var beam in redBeams)
            {
                switch (beam.Direction)
                {
                    case "left":
                        beam.Velocity.X = -beam.Speed;
                        break;
                    case "right":
                        beam.Velocity.X = beam.Speed;
                        break;
                    case "up":
                        beam.Velocity.Y = -beam.Speed;
                        break;
                    case "down":
                        beam.Velocity.Y = beam.Speed;
                        break;
                    default:
                        break;
                }
            }
            foreach (var beam in greenBeams)
            {
                switch (beam.Direction)
                {
                    case "left":
                        beam.Velocity.X = -beam.Speed;
                        break;
                    case "right":
                        beam.Velocity.X = beam.Speed;
                        break;
                    case "up":
                        beam.Velocity.Y = -beam.Speed;
                        break;
                    case "down":
                        beam.Velocity.Y = beam.Speed;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SlashingEnemies()
        {
            foreach (var Sword in sword)
            {
                for (int enemy = enemies.Count - 1; enemy >= 0; enemy--)
                {
                    if (Sword.IsTouchingRight(enemies[enemy]) ||
                        Sword.IsTouchingLeft(enemies[enemy]) ||
                        Sword.IsTouchingTop(enemies[enemy]) ||
                        Sword.IsTouchingBottom(enemies[enemy]))
                    {
                        enemies.RemoveAt(enemy);
                        isInvis = true;
                        playerSprite[0].Color = Color.Gold;
                        Score += 1;
                    }
                }
            }
        }

        private void SwordRemover(GameTime gameTime)
        {
            if (sword.Count > 0)
            {
                swordTimer += gameTime.ElapsedGameTime;
                if (swordTimer.TotalSeconds >= 0.25)
                {
                    sword.Clear();
                    swordTimer = TimeSpan.FromSeconds(0);
                }
            }
        }

        private void InvisibillityFrames(GameTime gameTime)
        {
            if (isInvis)
            {
                invisFramesTimer += gameTime.ElapsedGameTime;
                if (invisFramesTimer.TotalSeconds >= 2)
                {
                    isInvis = false;
                    playerSprite[0].Color = Color.White;
                    invisFramesTimer = TimeSpan.FromSeconds(0);
                }
            }
        }

        private void EnemyMovement(GameTime gameTime)
        {
            switch (allGenres[genreNum].name)
            {
                case "Platformer":
                    if ((enemySpeed >= 1f && enemySpeedChanger > 0) ||
                    (enemySpeed <= -1f && enemySpeedChanger < 0))
                        enemySpeedChanger = enemySpeedChanger * -1;
                    enemySpeed += enemySpeedChanger;
                    foreach (var enemy in enemies)
                    {
                        enemy.Velocity.Y = enemySpeed;
                    }
                    break;

                case "Adventure":
                    break;

                case "Space Shooter":
                    foreach (var enemy in enemies)
                    {
                        enemy.attackTimer += gameTime.ElapsedGameTime;
                        if (enemy.attackTimer.TotalSeconds >= enemy.attackFrequency)
                        {
                            System.Diagnostics.Debug.WriteLine("shoot");
                            enemy.attackTimer = TimeSpan.FromSeconds(0);
                            enemy.attackFrequency = rnd.Next(1, 6);
                            greenBeamDirectionNum = rnd.Next(1, 5);
                            switch (greenBeamDirectionNum)
                            {
                                case 1:
                                    beamDirection = "right";
                                    greenBeamStartPos = new Vector2(enemy.Rectangle.Right + 1, enemy.Rectangle.Y + 67);
                                    greenBeamTextureNum = 0;
                                    break;
                                case 2:
                                    beamDirection = "left";
                                    greenBeamStartPos = new Vector2(enemy.Rectangle.Left - 51, enemy.Rectangle.Y + 67);
                                    greenBeamTextureNum = 0;
                                    break;
                                case 3:
                                    beamDirection = "up";
                                    greenBeamStartPos = new Vector2(enemy.Rectangle.Left + 67, enemy.Rectangle.Y - 51);
                                    greenBeamTextureNum = 1;
                                    break;
                                case 4:
                                    beamDirection = "down";
                                    greenBeamStartPos = new Vector2(enemy.Rectangle.Left + 67, enemy.Rectangle.Bottom + 1);
                                    greenBeamTextureNum = 1;
                                    break;
                                default:
                                    break;
                            }
                            greenBeams.Add(new LazerBeam(greenBeamTextures[greenBeamTextureNum])
                            {
                                Position = greenBeamStartPos,
                                Color = Color.White,
                                Direction = beamDirection,
                                Speed = 15f,
                            });
                        }
                    }
                    break;
                default:
                    break;
            }
            
        }

        private void ScoreHandler()
        {
            switch (Score)
            {
                case 3:
                    numOfEnemies = 2;
                    break;
                case 10:
                    numOfEnemies = 3;
                    break;
                case 20:
                    numOfEnemies = 4;
                    break;
                case 30:
                    numOfEnemies = 5;
                    break;
                case 40:
                    numOfEnemies = 6;
                    break;
                case 50:
                    numOfEnemies = 7;
                    break;
                default:
                    break;
            }
        }

        private void RegenEnemis()
        {
            while (enemies.Count < numOfEnemies)
            {
                enemies.Add(new Enemy(allGenres[genreNum].enemy)
                {
                    Position = new Vector2(rnd.Next(0, (int)(baseScreenSize.X - 150f)), rnd.Next(151, (int)(baseScreenSize.Y - 150f))),
                    Color = new Color(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)),
                    Speed = 0f,
                    Input = virtualGamePad,
                    Direction = "right",
                    attackFrequency = rnd.Next(1, 6),
                });
            }
        }

        private void CollisionWithEnemy()
        {
            for (int enemy = enemies.Count - 1; enemy >= 0; enemy--)
            {
                if ((playerSprite[0].Velocity.X > 0 && playerSprite[0].IsTouchingLeft(enemies[enemy]) ||
                    playerSprite[0].Velocity.X < 0 && playerSprite[0].IsTouchingRight(enemies[enemy]) ||
                    playerSprite[0].Velocity.Y < 0 && playerSprite[0].IsTouchingBottom(enemies[enemy])) &
                    !isInvis)
                {
                    playerHP -= 1;
                    enemies.RemoveAt(enemy);
                    isInvis = true;
                    playerSprite[0].Color = Color.Gold;
                }

                //jumping on enemy
                else if (playerSprite[0].Velocity.Y > 0 && playerSprite[0].IsTouchingTop(enemies[enemy]))
                {
                    if (allGenres[genreNum].canUp == true)
                    {
                        playerHP -= 1;
                        enemies.RemoveAt(enemy);
                        isInvis = true;
                        playerSprite[0].Color = Color.Gold;
                    }
                    else
                    {
                        playerInAir = true;
                        hitJump = true;
                        playerJumpingSpeed = -7f;
                        Score += 1;
                        //var secondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        //enemies[enemy].Opacity = Math.Max(0.3f, enemies[enemy].Opacity - secondsElapsed * 2);
                        //if (enemies[enemy].Opacity <= 0)
                        //    enemies.RemoveAt(enemy);
                        enemies.RemoveAt(enemy);
                        isInvis = true;
                        playerSprite[0].Color = Color.Gold;
                    }
                }
            }
        }

        private void CollisionWithBorders()
        {
            if (playerSprite[0].Position.X < -150f)
                playerSprite[0].Position.X = _graphics.GraphicsDevice.Viewport.Width;
            if (playerSprite[0].Position.X > _graphics.GraphicsDevice.Viewport.Width)
                playerSprite[0].Position.X = -150f;

            if (playerSprite[0].Rectangle.Top <= 150f)
            {
                playerJumpingSpeed = 0;
                playerSprite[0].Velocity.Y = 0;
                playerSprite[0].Position.Y = 151f;
            }
            if (playerSprite[0].Rectangle.Bottom > _graphics.GraphicsDevice.Viewport.Height - 80)
            {
                playerSprite[0].Position.Y = _graphics.GraphicsDevice.Viewport.Height - 150 - 80;
                hitJump = false;
            }
        }

        private void CollisionWithPlatform()
        {
            for (int i = 0; i < 2; i++)
            {
                if ((playerJumpingSpeed != 0 || playerSprite[0].Velocity.Y > 0) && playerSprite[0].IsTouchingTop(platforms[i]))
                {
                    playerJumpingSpeed = 0;
                    playerSprite[0].Velocity.Y = 0;
                    playerOnPlat[i] = true;
                    hitJump = false;
                    playerInAir = false;
                    playerSprite[0].Position.Y = (platforms[0].Rectangle.Top - 150);
                    //System.Diagnostics.Debug.WriteLine("hit plat");
                }
                else
                {
                    playerInAir = true;
                }

                if ((playerJumpingSpeed != 0 || playerSprite[0].Velocity.Y < 0) && playerSprite[0].IsTouchingBottom(platforms[i]))
                {
                    playerJumpingSpeed = 0;
                    playerSprite[0].Velocity.Y = 0;
                    playerSprite[0].Position.Y = (platforms[0].Rectangle.Bottom + 1);
                }

                if (playerSprite[0].Velocity.X > 0 && playerSprite[0].IsTouchingLeft(platforms[i]) ||
                   playerSprite[0].Velocity.X < 0 && playerSprite[0].IsTouchingRight(platforms[i]))
                    playerSprite[0].Velocity.X = 0;
            }

            if ((playerOnPlat[0] && !playerOnPlat[1]) ||
                (!playerOnPlat[0] && playerOnPlat[1]))
                if (((playerSprite[0].Rectangle.Right <= _graphics.GraphicsDevice.Viewport.Width && playerSprite[0].Rectangle.Left > platforms[1].Rectangle.Right) ||
                   (playerSprite[0].Rectangle.Right < platforms[1].Rectangle.Left && playerSprite[0].Rectangle.Left > platforms[0].Rectangle.Right) ||
                   (playerSprite[0].Rectangle.Right < platforms[0].Rectangle.Left && playerSprite[0].Rectangle.Left >= 0)) &&
                   playerSprite[0].Rectangle.Bottom == platforms[0].Rectangle.Top)
                {
                    playerInAir = true;
                    hitJump = true;
                    playerOnPlat[0] = false;
                    playerOnPlat[1] = false;
                }

            //System.Diagnostics.Debug.WriteLine("{0}, {1}", playerOnPlat[0].ToString(), playerOnPlat[1].ToString());
        }

        private void TimerAndGenreChange(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime;
            fakeSeconds = (timeBetweenGames / 15);
            if (gameTimer.TotalSeconds >= fakeSeconds)
            {
                gameTimer = TimeSpan.FromSeconds(0);
                switcher++;
            }

            if (switcher == 15)
            {
                //gameTimer = TimeSpan.FromSeconds(0);
                timeBetweenGames -= 0.25;
                genreNum = nextGenre;
                nextGenre = rnd.Next(0, 3);
                while (nextGenre == genreNum)
                    nextGenre = rnd.Next(0, 3);
                switcher = 0;
                sword.Clear();
                redBeams.Clear();
                greenBeams.Clear();
                if (!allGenres[genreNum].canUp)
                    playerInAir = true;
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            // get all of our input states
            touchState = TouchPanel.GetState();
            gamePadState = virtualGamePad.GetState(touchState, GamePad.GetState(PlayerIndex.One));

#if !NETFX_CORE
            // Exit the game when back is pressed.
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Exit();
#endif

            GetInput(gamePadState);

            switch (whichBtn)
            {
                case "left":
                    playerSprite[0].Velocity.X = -playerSprite[0].Speed;
                    playerLastDirection = "left";
                    //playerLastDirectionNum = 2 * MathHelper.PiOver2;

                    swordStartPos = new Vector2(playerSprite[0].Rectangle.Left - 151, playerSprite[0].Rectangle.Y + 50);
                    swordTextureNum = 0;
                    foreach (var Sword in sword)
                        Sword.Velocity.X = -Sword.Speed;

                    beamStartPos = new Vector2(playerSprite[0].Rectangle.Left - 51, playerSprite[0].Rectangle.Y + 67);
                    redBeamTextureNum = 0;
                    break;

                case "right":
                    playerSprite[0].Velocity.X = playerSprite[0].Speed;
                    playerLastDirection = "right";
                    //playerLastDirectionNum = 0f;

                    swordStartPos = new Vector2(playerSprite[0].Rectangle.Right + 1, playerSprite[0].Rectangle.Y + 50);
                    swordTextureNum = 1;
                    foreach (var Sword in sword)
                        Sword.Velocity.X = Sword.Speed;

                    beamStartPos = new Vector2(playerSprite[0].Rectangle.Right + 1, playerSprite[0].Rectangle.Y + 67);
                    redBeamTextureNum = 0;
                    break;

                case "up":
                    if (allGenres[genreNum].canUp)
                        playerSprite[0].Velocity.Y = -playerSprite[0].Speed;
                    playerLastDirection = "up";
                    //playerLastDirectionNum = -MathHelper.PiOver2;

                    swordStartPos = new Vector2(playerSprite[0].Rectangle.X + 50, playerSprite[0].Rectangle.Top - 151);
                    swordTextureNum = 2;
                    foreach (var Sword in sword)
                        Sword.Velocity.Y = -Sword.Speed;

                    beamStartPos = new Vector2(playerSprite[0].Rectangle.Left + 67, playerSprite[0].Rectangle.Y - 51);
                    redBeamTextureNum = 1;
                    break;

                case "down":
                    if (allGenres[genreNum].canUp)
                        playerSprite[0].Velocity.Y = playerSprite[0].Speed;
                    playerLastDirection = "down";
                    //playerLastDirectionNum = MathHelper.PiOver2;

                    swordStartPos = new Vector2(playerSprite[0].Rectangle.X + 50, playerSprite[0].Rectangle.Bottom + 1);
                    swordTextureNum = 3;
                    foreach (var Sword in sword)
                        Sword.Velocity.Y = Sword.Speed;

                    beamStartPos = new Vector2(playerSprite[0].Rectangle.Left + 67, playerSprite[0].Rectangle.Bottom + 1);
                    redBeamTextureNum = 1;
                    break;

                case "action":
                    continuePressed = true;
                    //do action by genre
                    switch (allGenres[genreNum].abillity)
                    {
                        case "Jump":
                            if (!playerInAir)
                            {
                                playerOnPlat[0] = false;
                                playerOnPlat[1] = false;
                                //System.Diagnostics.Debug.WriteLine("jumping");
                                playerInAir = true;
                                hitJump = true;
                                playerJumpingSpeed = -23f;
                            }
                            break;
                        case "Sword":
                            if (sword.Count == 0)
                            {
                                sword.Add(new Sword(swordTextures[swordTextureNum])
                                {
                                    Position = swordStartPos,
                                    Color = Color.White,
                                    Speed = 5f,
                                    Input = virtualGamePad,
                                    Direction = "right",
                                    Rotation = 0f,
                                    //Opacity = 1f,
                                });
                            }
                            break;
                        case "Shoot":
                            if (canShoot)
                            {
                                redBeams.Add(new LazerBeam(redBeamTextures[redBeamTextureNum])
                                {
                                    Position = beamStartPos,
                                    Color = Color.White,
                                    Direction = playerLastDirection,
                                    Speed = 15f,
                                });
                                canShoot = false;
                                System.Diagnostics.Debug.WriteLine("pew pew");
                            }
                            break;
                        default:
                            break;
                    }


                    break;
                default:
                    continuePressed = false;
                    break;
            }

            //System.Diagnostics.Debug.WriteLine(continuePressed);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
                if (playerHP == 0)
                {
                    //do game over
                }
            }

            wasContinuePressed = continuePressed;

            virtualGamePad.Update(gameTime);
        }

        private void GetInput(GamePadState gamePadState)
        {
            if (gamePadState.IsButtonDown(Buttons.DPadLeft))
                whichBtn = "left";
            else if (gamePadState.IsButtonDown(Buttons.DPadRight))
                whichBtn = "right";
            else if (gamePadState.IsButtonDown(Buttons.DPadUp))
                whichBtn = "up";
            else if (gamePadState.IsButtonDown(Buttons.DPadDown))
                whichBtn = "down";
            if (gamePadState.IsButtonDown(Buttons.A))
                whichBtn = "action";

            if (!gamePadState.IsButtonDown(Buttons.DPadLeft) &&
                !gamePadState.IsButtonDown(Buttons.DPadRight) &&
                !gamePadState.IsButtonDown(Buttons.DPadUp) &&
                !gamePadState.IsButtonDown(Buttons.DPadDown) &&
                !gamePadState.IsButtonDown(Buttons.A))
                whichBtn = "none";
        }

        public void PlayerJump()
        {
            playerJumpingSpeed += 0.5f;
            playerSprite[0].Velocity.Y = playerJumpingSpeed;
            if (playerSprite[0].Rectangle.Bottom == _graphics.GraphicsDevice.Viewport.Height - 80)
                playerInAir = false;
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here

            var widthHalf = _graphics.GraphicsDevice.Viewport.Width / 2;
            var heightMax = _graphics.GraphicsDevice.Viewport.Height;
            var heightHalf = heightMax / 2;
            var timerSize = new Vector2(font.MeasureString(fakeTime[switcher].ToString()).X / 2, font.MeasureString(fakeTime[switcher].ToString()).Y / 2);
            var timerArea = new Vector2(widthHalf, 75);
            var scoreSize = new Vector2(0, font.MeasureString("SCORE").Y / 2);
            var scoreArea = new Vector2(50, 75);
            var healthVector2 = new Vector2((_graphics.GraphicsDevice.Viewport.Width - 366), 0);
            var nextVector2 = new Vector2((healthVector2.X - smallFont.MeasureString("Next:").X - 100), (scoreArea.Y - scoreSize.Y));
            var nextGenreVector2 = new Vector2( (nextVector2.X + (smallFont.MeasureString("Next:").X / 2) - (smallFont.MeasureString(allGenres[nextGenre].name).X / 2)), ((timerArea - timerSize).Y + font.MeasureString(fakeTime[switcher].ToString()).Y - smallFont.MeasureString(allGenres[nextGenre].name).Y));

            _spriteBatch.Begin();
            _spriteBatch.Draw(allGenres[genreNum].background, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            foreach (var sprite in platforms)
                sprite.Draw(_spriteBatch);
            foreach (var sprite in enemies)
                sprite.Draw(_spriteBatch);
            foreach (var sprite in playerSprite)
                sprite.Draw(_spriteBatch);
            foreach (var sprite in sword)
                sprite.Draw(_spriteBatch);
            foreach (var sprite in redBeams)
                sprite.Draw(_spriteBatch);
            foreach (var sprite in greenBeams)
                sprite.Draw(_spriteBatch);



            _spriteBatch.Draw(scoreBarSprite, new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(font, fakeTime[switcher].ToString(), (timerArea - timerSize), Color.Red);
            //_spriteBatch.DrawString(font, "NEXT GENRE:", new Vector2((widthHalf + 100), 0), null, Color.White, 0f, 0.5f);
            _spriteBatch.DrawString(font, "SCORE:"+ Score,(scoreArea - scoreSize), Color.White);
            _spriteBatch.Draw(playerHPTextures[playerHP], healthVector2, Color.White);
            _spriteBatch.DrawString(smallFont, "Next:", nextVector2, Color.White);
            _spriteBatch.DrawString(smallFont, allGenres[nextGenre].name, nextGenreVector2, Color.White);


            if (touchState.IsConnected)
                virtualGamePad.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
