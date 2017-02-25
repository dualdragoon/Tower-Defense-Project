﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Duality;
using Duality.Interaction;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace Tower_Defense_Project
{
    sealed class MultiValue<V1, V2> : IEquatable<MultiValue<V1, V2>>
    {
        public V1 Value1 { get; private set; }
        public V2 Value2 { get; private set; }

        public MultiValue(V1 v1, V2 v2)
        {
            Value1 = v1;
            Value2 = v2;
        }

        public bool Equals(MultiValue<V1, V2> other)
        {
            if (other == null)
            {
                return false;
            }
            return EqualityComparer<V1>.Default.Equals(this.Value1, other.Value1) &&
                   EqualityComparer<V2>.Default.Equals(this.Value2, other.Value2);
        }

        public override bool Equals(object o)
        {
            return Equals(o as MultiValue<V1, V2>);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<V1>.Default.GetHashCode(Value1) * 37 +
                   EqualityComparer<V2>.Default.GetHashCode(Value2);
        }
    }
    class Menu
    {
        Main baseMain;
        string lastMenu = "Main Menu";
        Texture2D background;
        Vector2 intendedResolution;

        List<Button> buttons = new List<Button>();
        List<ButtonType> buttonTypes = new List<ButtonType>();
        Dictionary<Button, List<MultiValue<int, string>>> buttonLefts = new Dictionary<Button, List<MultiValue<int, string>>>();
        Dictionary<Button, List<MultiValue<int, string>>> buttonRights = new Dictionary<Button, List<MultiValue<int, string>>>();
        //List<int> buttonLeftResults = new List<int>();
        //List<int> buttonRightResults = new List<int>();
        //List<string> buttonLeftDestinations = new List<string>();
        //List<string> buttonRightDestinations = new List<string>();
        List<Texture2D> textures = new List<Texture2D>();
        List<Vector2> textureLocations = new List<Vector2>();

        public string LastMenu
        {
            get { return lastMenu; }
        }

        public Menu(Main main)
        {
            baseMain = main;
            LoadMenu("Main Menu");
        }

        public void LoadMenu(string menuName)
        {
            try
            {
                XmlDocument read = new XmlDocument();
                read.Load("Content/Menus/" + menuName + ".xml");
                XmlNode node = read.SelectSingleNode("/Menu");
                XmlNode res = node.SelectSingleNode("IntendedResolution");
                XmlNode resX = res.SelectSingleNode("X"), resY = res.SelectSingleNode("Y");
                intendedResolution = new Vector2(float.Parse(resX.InnerText), float.Parse(resY.InnerText));

                foreach (XmlNode i in node)
                {
                    if (i.Name == "BackgroundTexture") background = Main.GameContent.Load<Texture2D>("Menus/" + menuName + "/" + i.InnerText + ".png");
                    else if (i.Name == "Texture")
                    {
                        foreach (XmlNode l in i)
                        {
                            if (l.Name == "TextureName") textures.Add(Main.GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png"));
                            else if (l.Name == "Position")
                            {
                                XmlNode x = l.SelectSingleNode("X"), y = l.SelectSingleNode("Y");
                                textureLocations.Add(new Vector2((float.Parse(x.InnerText) / intendedResolution.X) * Main.Scale.X, (float.Parse(y.InnerText) / intendedResolution.Y) * Main.Scale.Y));
                            }
                        }
                    }
                    else if (i.Name == "Button")
                    {
                        List<int> buttonLeftResults = new List<int>();
                        List<int> buttonRightResults = new List<int>();
                        List<string> buttonLeftDestinations = new List<string>();
                        List<string> buttonRightDestinations = new List<string>();

                        ButtonType b = ButtonType.Ellipse;
                        Texture2D normal = Main.GameContent.Load<Texture2D>("Textures/help"), hovered = Main.GameContent.Load<Texture2D>("Textures/help");
                        Vector2 pos = Vector2.Zero;
                        float w = 0, h = 0, diameter = 0;
                        int leftResultType = 0, rightResultType = 0;
                        foreach (XmlNode l in i)
                        {
                            if (l.Name == "ButtonType")
                            {
                                b = (ButtonType)int.Parse(l.InnerText);
                                buttonTypes.Add(b);
                            }
                            else if (l.Name == "NormalTexture") normal = Main.GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "HoveredTexture") hovered = Main.GameContent.Load<Texture2D>("Menus/" + menuName + "/" + l.InnerText + ".png");
                            else if (l.Name == "Position")
                            {
                                XmlNode x = l.SelectSingleNode("X"), y = l.SelectSingleNode("Y");
                                pos = new Vector2(float.Parse(x.InnerText), float.Parse(y.InnerText));
                            }
                            else if (l.Name == "LeftClickResult")
                            {
                                XmlNode type = l.SelectSingleNode("ResultType"), name = l.SelectSingleNode("ResultName");
                                leftResultType = int.Parse(type.InnerText);
                                buttonLeftResults.Add(leftResultType);
                                buttonLeftDestinations.Add(name.InnerText);
                            }
                            else if (l.Name == "RightClickResult")
                            {
                                XmlNode type = l.SelectSingleNode("ResultType"), name = l.SelectSingleNode("ResultName");
                                rightResultType = int.Parse(type.InnerText);
                                buttonRightResults.Add(rightResultType);
                                buttonRightDestinations.Add(name.InnerText);
                            }
                            else if (b == ButtonType.Rectangle)
                            {
                                if (l.Name == "Size")
                                {
                                    XmlNode width = l.SelectSingleNode("Width"), height = l.SelectSingleNode("Height");
                                    w = (float.Parse(width.InnerText) / intendedResolution.X) * Main.Scale.X;
                                    h = (float.Parse(height.InnerText) / intendedResolution.Y) * Main.Scale.Y;
                                }
                            }
                            else if (b == ButtonType.Circle)
                            {
                                if (l.Name == "Diameter") diameter = float.Parse(l.InnerText);
                            }
                        }
                        pos = new Vector2((pos.X / intendedResolution.X) * Main.Scale.X, (pos.Y / intendedResolution.Y) * Main.Scale.Y);
                        if (b == ButtonType.Rectangle) buttons.Add(new Button(pos, (int)w, (int)h, buttons.Count, Main.CurrentMouse, normal, hovered, true, Main.Scale.X, Main.Scale.Y));
                        else if (b == ButtonType.Circle) buttons.Add(new Button(pos, diameter, buttons.Count, Main.CurrentMouse, normal, hovered, true, Main.Scale.X, Main.Scale.Y));
                        else if (b == ButtonType.Ellipse) buttons.Add(new Button(pos, buttons.Count, Main.CurrentMouse, normal, hovered, true, Main.Scale.X, Main.Scale.Y));

                        List<MultiValue<int, string>> firstValues = new List<MultiValue<int, string>>();
                        for (int l = 0; l < buttonLeftResults.Count; l++)
                        {
                            firstValues.Add(new MultiValue<int, string>(buttonLeftResults[l], buttonLeftDestinations[l]));
                        }
                        buttonLefts.Add(buttons[buttons.Count - 1], firstValues);

                        List<MultiValue<int, string>> secondValues = new List<MultiValue<int, string>>();
                        for (int l = 0; l < buttonRightResults.Count; l++)
                        {
                            secondValues.Add(new MultiValue<int, string>(buttonRightResults[l], buttonRightDestinations[l]));
                        }
                        buttonRights.Add(buttons[buttons.Count - 1], secondValues);
                    }
                }

                foreach (Button i in buttons)
                {
                    #region LeftClickResults
                    List<MultiValue<int, string>> leftValues = buttonLefts[i];
                    for (int l = 0; l < leftValues.Count; l++)
                    {
                        MultiValue<int, string> value = leftValues[l];
                        if (value.Value1 == 0)
                        {
                            i.LeftClicked += (object sender, EventArgs e) =>
                            {
                                string name = value.Value2;
                                background = Main.GameContent.Load<Texture2D>("Textures/help");
                                Clear();
                                lastMenu = menuName;
                                LoadMenu(name);
                            };
                        }
                        else if (value.Value1 == 1)
                        {
                            i.LeftClicked += (object sender, EventArgs e) =>
                            {
                                string name = value.Value2;
                                if (name != "null")
                                {
                                    Clear();
                                    lastMenu = menuName;
                                    baseMain.LevelName = name;
                                    baseMain.CurrentState = GameState.Play;
                                }
                            };
                        }
                        else if (value.Value1 == 2)
                        {
                            i.LeftClicked += (object sender, EventArgs e) =>
                            {
                                Clear();
                                lastMenu = menuName;
                                baseMain.CurrentState = GameState.LevelDesigner;
                            };
                        } 
                    }
                    #endregion

                    #region RightClickResults
                    List<MultiValue<int, string>> rightValues = buttonRights[i];
                    for (int l = 0; l < rightValues.Count; l++)
                    {
                        MultiValue<int, string> value = rightValues[l];
                        if (value.Value1 == 0)
                        {
                            i.RightClicked += (object sender, EventArgs e) =>
                            {
                                string name = value.Value2;
                                background = Main.GameContent.Load<Texture2D>("Textures/help");
                                Clear();
                                lastMenu = menuName;
                                LoadMenu(name);
                            };
                        }
                        else if (value.Value1 == 1)
                        {
                            i.RightClicked += (object sender, EventArgs e) =>
                            {
                                string name = value.Value2;
                                if (name != "null")
                                {
                                    Clear();
                                    lastMenu = menuName;
                                    baseMain.LevelName = name;
                                    baseMain.CurrentState = GameState.Play;
                                }
                            };
                        }
                        else if (value.Value1 == 2)
                        {
                            i.RightClicked += (object sender, EventArgs e) =>
                            {
                                Clear();
                                lastMenu = menuName;
                                baseMain.CurrentState = GameState.LevelDesigner;
                            };
                        } 
                    }
                    #endregion
                }
            }
            catch (AssetNotFoundException e)
            {
                ErrorHandler.RecordError(2, 007, "A specified assest was not found, this could be due to it not existing, being a wrong format, or being misnamed.", e.Message);
                Console.WriteLine("There was a problem loading an asset. Full details on this error can be found in the error log.");
            }
            catch (FileNotFoundException e)
            {
                ErrorHandler.RecordError(3, 001, "The destination file could not be found; make sure that files are named correctly and that they are correctly referenced in other menus. If this is happening at start up make sure that the Menu.xml is correctly named and present.", e.Message);
            }
        }

        private void Clear()
        {
            buttons.Clear();
            buttonTypes.Clear();
            buttonLefts.Clear();
            buttonRights.Clear();
            textures.Clear();
            textureLocations.Clear();
        }

        public void Update(GameTime gameTime)
        {
            Main.CurrentMouse = Main.Mouse.GetState();

            try
            {
                foreach (Button i in buttons) i.Update(Main.CurrentMouse);
            }
            catch { }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RectangleF rect = new RectangleF(0, 0, Main.Scale.X, Main.Scale.Y);
            spriteBatch.Draw(background, rect, Color.White);

            for (int i = 0; i < textures.Count; i++)
            {
                Vector2 textureSize = new Vector2((textures[i].Width / intendedResolution.X) * Main.Scale.X, (textures[i].Height / intendedResolution.Y) * Main.Scale.Y),
                    textureLocation = textureLocations[i];
                spriteBatch.Draw(textures[i], new RectangleF(textureLocation.X, textureLocation.Y, textureSize.X, textureSize.Y), Color.White);
            }

            foreach (Button i in buttons)
            {
                if (buttonTypes[i.ButtonNum] == ButtonType.Rectangle)
                {
                    spriteBatch.Draw(i.Texture, i.Collision, Color.White);
                }
                else
                {
                    spriteBatch.Draw(i.Texture, i.Position, Color.White);
                }
            }
        }
    }
}