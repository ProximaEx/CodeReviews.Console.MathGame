using System;
using System.Collections.Generic;

namespace mathGameByPEx
{
	class Program
	{
		static void Main(string[] args)
		{
			// Sets window to predetermined size if possible
			int targetW = 60;
			int targetH = 28;
			int width = (targetW > Console.LargestWindowWidth) ? Console.LargestWindowWidth : targetW;
			int height = (targetH > Console.LargestWindowHeight) ? Console.LargestWindowHeight : targetH;
			if (OperatingSystem.IsWindows())
			{
				Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
				Console.SetWindowSize(width, height);
				Console.SetBufferSize(width, height);
			}
			else
			{
				width = Console.WindowWidth;
				height = Console.WindowHeight;
			}

			const int menuLineWidth = 36;
			const int maxScoreLen = 6;

			bool menuLoop;
			bool start;
			bool settings;
			bool highs;
			bool escaped = false;
			bool egg = false;

			string[] gameModes = { "+ Add +", "- Subtract -", "× Multiply ×", "/ Divide /", "? Random ?" };
			int gameMode = 0;
			int questionQuantity = 5;
			int maxNumber = 12;

			// Features not added yet
			string clockState = "Off";
			string difficulty = "Easy";

			// High scores are stored as strings, 9 char in length with 6 digit score
			// (constrained by maxScoreLen) first for sorting, followed by 3 char name
			List<string> highScores = new();

			// Example high scores
			highScores.Add("   245HOL");
			highScores.Add("    15OOF");
			highScores.Add("  1000YSH");
			highScores.Add("    65PEX");
			highScores.Add("    80BAB");
			highScores.Sort();
			highScores.Reverse();

			MathGame();

			void MathGame()
			{
				while (escaped == false)
				{
					DrawMainMenu();
					ModeSelector();
					if (settings) { ShowSettings(); }
					else if (highs) { ShowHighScores(); }
					else if (escaped) { Exit(); }
					else if (start) { StartRound(); }
				}
			}

			void DrawMainMenu()
			{
				string[] menuLines = {
				"\n",
				"____________________________________",
				"|__________________________________|",
				"||                                ||",
				"||  __    __    __ ______ __  __  ||",
				"||  |\\\\  /||   /|| |[[[[| ||  ||  ||",
				"||  ||\\\\//||  //||   ||   ||==||  ||",
				"||  || \\/ || //=||   ||   ||  ||  ||",
				"||  ||    ||//  ||   ||   ||  ||  ||",
				"||  ``    ```   ``   ``   ``  ``  ||",
				"||_____________ Game _____________||",
				"|__________________________________|",
				"\n\n",
				"        _ select game mode _        ",
				"        |                  |        ",
				"        |                  |        ",
				"        |__________________|        ",
				"\n",
				"      Start          > Enter <      ",
				"      High Scores    >  'H'  <      ",
				"      Settings       >  'S'  <      ",
				"      Exit           >  Esc  <      "
				};
				Console.Clear();
				Console.SetCursorPosition(0, 0);
				foreach (string m in menuLines)
				{
					Console.WriteLine(m.PadLeft(width / 2 + menuLineWidth / 2));
				}
			}

			void ModeSelector()
			{
				start = false;
				menuLoop = true;
				settings = false;
				highs = false;

				while (menuLoop)
				{
					int modeBoxW = 16;
					int gameModeWidth = gameModes[gameMode].Length;
					string[] gameModeClearSet = { "                ",
					(gameModes[gameMode].PadLeft(gameModeWidth + (modeBoxW - gameModeWidth) / 2)) };

					for (int i = 0; i < 2; i++)
					{
						Console.SetCursorPosition((width - menuLineWidth) / 2 + 10, 18);
						Console.WriteLine(gameModeClearSet[i]);
					}

					ConsoleKey startKey = Console.ReadKey(true).Key;
					switch (startKey)
					{
						case ConsoleKey.Escape:
							{ menuLoop = false; escaped = true; break; }
						case ConsoleKey.Enter:
							{ menuLoop = false; start = true; break; }
						case ConsoleKey.S:
							{ menuLoop = false; settings = true; break; }
						case ConsoleKey.H:
							{ menuLoop = false; highs = true; break; }
						case ConsoleKey.NumPad7:
							{ menuLoop = false; egg = true; break; }
						case ConsoleKey.LeftArrow:
							{ gameMode--; break; }
						case ConsoleKey.DownArrow:
							{ gameMode--; break; }
						default:
							{ gameMode++; break; }
					}
					gameMode = (gameMode + gameModes.Length) % (gameModes.Length);
				}
				if (egg) { UnskippableCutscene(); }
			}

			void ShowSettings()
			{
				Console.Clear();
				string[] menuLines = {
				"\n\n\n\n",
				"           ______________           ",
				"__________[   Settings:  ]__________",
				"|__________________________________|",
				"||                                ||",
				"||                                ||",
				"||   Maximum number:   [      ]   ||",
				"||                                ||",
				"||   # of Questions:   [      ]   ||",
				"||                                ||",
				"||   Clock N/A:        [      ]   ||",
				"||                                ||",
				"||   Difficulty N/A:   [      ]   ||",
				"||                                ||",
				"||            [ Exit ]            ||",
				"||                                ||",
				"||________________________________||",
				"|__________________________________|"
				};
				foreach (string m in menuLines)
				{
					Console.WriteLine(m.PadLeft(width / 2 + menuLineWidth / 2));
				}
				int columnX = (width - menuLineWidth) / 2 + 23;

				int[] optXCoords = { columnX, columnX, columnX, columnX, columnX - 9 };
				int[] optYCoords = { 10, 12, 14, 16, 18 };

				int selection = 0;
				bool exitSettings = false;

				while (!exitSettings)
				{
					bool optionSelected = false;

					while (!optionSelected)
					{
						// Loop to write current settings values
						string[] currentSettings = { Convert.ToString(maxNumber), Convert.ToString(questionQuantity), clockState, difficulty };
						for (int j = 0; j < optXCoords.Length - 1; j++)
						{
							for (int k = 0; k < 2; k++)
							{
								// Doing this twice fixes a write bug when escaped from field input
								Console.SetCursorPosition(optXCoords[j] + 2, optYCoords[j]);
								Console.Write(currentSettings[j].PadLeft(4));
							}
						}

						// Selector loop
						Console.SetCursorPosition(optXCoords[selection] + 1, optYCoords[selection]);
						ConsoleKey input = Console.ReadKey(true).Key;
						switch (input)
						{
							case ConsoleKey.Escape:
								{
									exitSettings = true;
									optionSelected = true;
									break;
								}
							case ConsoleKey.Enter:
								{
									optionSelected = true;
									if (selection == optXCoords.Length - 1) { exitSettings = true; }
									break;
								}
							case ConsoleKey.UpArrow:
								{
									selection--;
									selection = (selection + optXCoords.Length) % optXCoords.Length;
									break;
								}
							case ConsoleKey.LeftArrow:
								{
									selection--;
									selection = (selection + optXCoords.Length) % optXCoords.Length;
									break;
								}
							default:
								{
									selection++;
									selection = (selection + optXCoords.Length) % optXCoords.Length;
									break;
								}
						}
					}
					if (exitSettings) { break; }

					bool enterPressed = false;
					if (selection < 2)
					{
						// Clears previous value field
						Console.Write("      ");

						char[] setValue = { ' ' };
						ConsoleKeyInfo keyPress;

						for (int i = 0; i < 4; i++)
						{
							Console.SetCursorPosition(optXCoords[selection] + 2 + i, optYCoords[selection]);
							keyPress = Console.ReadKey(false);
							switch (keyPress.Key)
							{
								case ConsoleKey.Backspace:
									{
										i -= 2;
										break;
									}
								case ConsoleKey.Escape:
									{
										i = 4;
										Console.SetCursorPosition(optXCoords[selection] + 2, optYCoords[selection]);
										Console.Write("    ");
										Array.Clear(setValue);
										break;
									}
								case ConsoleKey.Enter:
									{
										enterPressed = true;
										break;
									}
								default:
									{
										char digit = keyPress.KeyChar;
										if ("0123456789".Contains(digit))
										{
											Array.Resize(ref setValue, i + 1);
											setValue[i] = digit;
										}
										else { i--; }
										break;
									}
							}
							if (enterPressed) { break; }
						}
						if (setValue[0] != 0)
						{
							string newValueString = string.Concat(setValue);
							int.TryParse(newValueString, out int newValue);
							if (selection == 0) { maxNumber = newValue; }
							else if (selection == 1) { questionQuantity = newValue; }
						}
					}
					// else functionality for clock setting goes here
					// else functionality for difficulty goes here
				}
			}

			void ShowHighScores()
			{
				Console.Clear();
				int scoresWidth = 18;
				int highScoresToDisplay;

				string[] menuLines = {
				"\n\n",
				"           ______________           ",
				"__________[ High Scores: ]__________",
				"|__________________________________|",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||                                ||",
				"||________________________________||",
				"|__________________________________|",
				"\n\n",
				"       Press any key to exit.       "
				};
				foreach (string m in menuLines)
				{
					Console.WriteLine(m.PadLeft(width / 2 + menuLineWidth / 2));
				}

				if (highScores.Count > 10) { highScoresToDisplay = 10; }
				else { highScoresToDisplay = highScores.Count; }

				for (int i = 0; i < highScoresToDisplay; i++)
				{
					string place = Convert.ToString(i + 1);
					Console.SetCursorPosition((width - menuLineWidth) / 2 + (menuLineWidth - scoresWidth) / 2, 7 + i);
					Console.Write(place + ". " + highScores[i].Substring(maxScoreLen) + " ");
					// Magic 6 below adjusts for name length + 3 chars in code line above
					string nonPaddedScore = highScores[i].Substring(0, maxScoreLen);
					nonPaddedScore = nonPaddedScore.Trim();
					Console.Write(" ".PadLeft(scoresWidth - 6 - place.Length - nonPaddedScore.Length - 1, '-'));
					Console.Write(" " + nonPaddedScore);
				}
				Console.ReadKey(true);
			}

			void StartRound()
			{
				Random random = new();
				char[] symbols = { '+', '-', '×', '/' };
				int questionCycles = 1;
				int numberOfQuestions = questionQuantity;
				bool randGame = false;

				int num1 = 0;
				int num2 = 0;

				bool backToMain = false;
				int correctAnswer = 0;
				string? input;
				int answer = 0;
				int thisScore = 0;

				// Random game protocol
				if (gameMode == 4)
				{
					questionCycles = numberOfQuestions;
					numberOfQuestions = 1;
					randGame = true;
				}

				// Question loops
				for (int i = 0; i < questionCycles; i++)
				{
					if (backToMain) { break; }
					if (randGame) { gameMode = random.Next(0, 4); }

					for (int j = 0; j < numberOfQuestions; j++)
					{
						// Generates question params and correct answer based on game mode
						switch (gameMode)
						{
							case 0:
								{
									do
									{ // Ensures sum is <= max number
										num1 = random.Next(1, maxNumber + 1);
										num2 = random.Next(1, maxNumber + 1);
									} while (num1 + num2 > maxNumber);
									correctAnswer = num1 + num2;
									break;
								}
							case 1:
								{
									num1 = 0;
									while (num1 <= num2)
									{ // Ensures random 1 bigger than random 2
										num1 = random.Next(1, maxNumber + 1);
										num2 = random.Next(1, maxNumber + 1);
									}
									correctAnswer = num1 - num2;
									break;
								}
							case 2:
								{
									num1 = 0;
									while (num1 * num2 == 0 || num1 * num2 > maxNumber)
									{ // Ensures product is <= maxNumber
										num1 = random.Next(1, maxNumber + 1);
										num2 = random.Next(1, maxNumber + 1);
									}
									correctAnswer = num1 * num2;
									break;
								}
							case 3:
								{
									do
									{ // Ensures random 2 is factor of random 1
										num2 = random.Next(1, maxNumber / 2 + 1);
										for (int k = 0; k < 5; k++)
										{
											num1 = random.Next(2, maxNumber + 1);
											if (num1 % num2 == 0 && num1 > num2) { break; }
										}
									} while (num1 % num2 != 0 || num1 <= num2);
									correctAnswer = num1 / num2;
									break;
								}
						}

						// Writes question
						Console.Clear();
						Console.SetCursorPosition(0, height / 4);
						string quest = $"{num1} {symbols[gameMode]} {num2}?";
						Console.WriteLine(quest.PadLeft(width / 2 + quest.Length / 2));

						bool answerValid = false;

						while (!answerValid)
						{
							// Draws answer box
							Console.SetCursorPosition(width / 2 - 5, height / 2 - 2);
							int answerBoxX = Console.CursorLeft;
							Console.WriteLine(" ________ ");
							Console.WriteLine("|        |".PadLeft(answerBoxX + 10));
							Console.WriteLine("|        |".PadLeft(answerBoxX + 10));
							Console.Write("|________|".PadLeft(answerBoxX + 10));

							Console.SetCursorPosition(width / 2 - 2, height / 2);
							input = Console.ReadLine();
							answerValid = int.TryParse(input, out answer);
							if (!answerValid && input != null && input.Equals("exit")) { backToMain = true; break; }
							if (!answerValid) { Console.WriteLine("\n\n\n" + "Please enter a number.".PadLeft(width / 2 + 11)); }
						}
						if (backToMain) { break; }

						// Generates correct or incorrect message
						if (correctAnswer == answer)
						{
							Console.WriteLine("\n\n\n" + "       Correct!       ".PadLeft(width / 2 + 11));
							thisScore += 10;
						}
						else
						{
							Console.WriteLine("\n\n\n" + "      Incorrect.      ".PadLeft(width / 2 + 11));
							thisScore -= 5;
						}

						// Pause before clearing for next question
						Thread.Sleep(900);
					}
				}
				if (backToMain) { return; }

				EndOfRound(thisScore);
			}

			void EndOfRound(int scoreIn)
			{
				string? keyPress;
				string playerName;

				Console.Clear();
				Console.SetCursorPosition(0, height / 2 - 6);
				int eorTop = Console.CursorTop;
				string[] eor = { "End of Round!", "", $"Your Score: {scoreIn}\n\n" };
				for (int i = 0; i < eor.Length; i++) { Console.WriteLine(eor[i].PadLeft(width / 2 + eor[i].Length / 2)); }

				if (scoreIn >= 0)
				{
					Console.WriteLine("Enter 3 digit name".PadLeft(width / 2 + 9));
					char[] playerChars = { ' ', ' ', ' ' };

					for (int i = 0; i < 3; i++)
					{
						Console.SetCursorPosition((width / 2 - 2) + i, eorTop + 7);
						keyPress = Console.ReadKey(false).Key.ToString();
						if (keyPress == null) { i--; }
						else if (keyPress.Length != 1) { i--; }
						else { playerChars[i] = char.Parse(keyPress); }
					}

					playerName = string.Concat(playerChars);
					string scoreToAdd = Convert.ToString(scoreIn);
					scoreToAdd = scoreToAdd.PadLeft(maxScoreLen).Insert(maxScoreLen, playerName);

					highScores.Add(scoreToAdd);
					highScores.Sort();
					highScores.Reverse();
				}
				else { Console.WriteLine("\n" + "Do better.".PadLeft(width / 2 + 5)); }

				Console.WriteLine("\n\n" + "...Back to the menu ".PadLeft(width / 2 + 10));
				Thread.Sleep(2000);
			}

			void Exit()
			{
				Console.Clear();
				Console.WriteLine("\n\n\n\n\n\n\n\n\n\n" + "Goodbye!".PadLeft(width / 2 + 4));
				Thread.Sleep(3000);
			}

			void UnskippableCutscene()
			{
				int theUltimateAnswerToLifeTheUniverseAndEverything = 42;

				string[,] scenes =
				{
					{ // Pause
						"                     ",
						"                     ",
						"                     ",
						" Oh...               ",
						"                     ",
						"                     ",
						"        you found it.",
						"                     ",
						"                     ",
						"                     ",
					},
					{ // Pause
						"                     ",
						"        .---.        ",
						"      ./     \\.      ",
						"    ./         \\.    ",
						"    !  /\\   /\\  !    ",
						"    |\\//\\\\ //\\\\/|    ",
						"    |\\/  \\'/  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"        .---.        ",
						"      ./     \\.      ",
						"    ./         \\.    ",
						"    !  /\\   /\\  !    ",
						"    '\\/  \\ /  \\/'    ",
						"    ,  /\\ ' /\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"      ./     \\.      ",
						"    ./         \\.    ",
						"    !  /\\   /\\  !    ",
						"    '\\/  \\ /  \\/'    ",
						"          '          ",
						"    ,  /\\   /\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"    ./         \\.    ",
						"    !  /\\   /\\  !    ",
						"    '\\/  \\ /  \\/'    ",
						"          '          ",
						"                     ",
						"    ,  /\\___/\\  ,    ",
						"    |\\/  \\_/  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"    !  /\\   /\\  !    ",
						"    '\\/  \\ /  \\/'    ",
						"          '          ",
						"                     ",
						"        n___n        ",
						"    ,  /\\-_-/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"    '\\/  \\ /  \\/'    ",
						"          '          ",
						"                     ",
						"        n___n        ",
						"        ('-')        ",
						"    ,  /\\   /\\  ,    ",
						"    |\\/  \\-/  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"          '          ",
						"        _   _        ",
						"        \\\\_//        ",
						"        ('-')        ",
						"        /   \\        ",
						"    ,  /\\.-./\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{ // Pause
						"        n   n        ",
						"        \\\\_//        ",
						"        ('-')        ",
						"        /   \\        ",
						"       ( .-. )       ",
						"    ,  /\\: :/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{ // Pause
						"        n   n        ",
						"        \\\\_//        ",
						"        ('-')        ",
						"  Happy /   \\        ",
						"       ( .-. )       ",
						"    ,  /\\: :/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{ // Pause
						"        n   n        ",
						"        \\\\_//        ",
						"        ('-')        ",
						"  Happy /   \\ Easter ",
						"       ( .-. )       ",
						"    ,  /\\: :/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{
						"        n   n        ",
						"        \\\\_//        ",
						"        ('-^)        ",
						"  Happy /   \\ Easter ",
						"       ( .-. )       ",
						"    ,  /\\: :/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{ // Pause
						"        n   n        ",
						"        \\\\_//        ",
						"        ('-')        ",
						"  Happy /   \\ Easter ",
						"       ( .-. )       ",
						"    ,  /\\: :/\\  ,    ",
						"    |\\/  \\ /  \\/|    ",
						"     \\    '    /     ",
						"      '._____.'      ",
						"                     ",
					},
					{ // Pause
						"                     ",
						"\"Ever tried.         ",
						" Ever failed.        ",
						" No matter.          ",
						" Try Again.          ",
						" Fail again.         ",
						" Fail better.\"       ",
						"                     ",
						"     -Samuel Beckett ",
						"                     ",
					},
					{ // Pause
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
						"                     ",
					},
					{ // Pause
						"      . _, _,~.~     ",
						"   .,'\\,~'_,~'~'~~~  ",
						"   ,/',/ __  __\\'    ",
						"   |---(  o|  o)|    ",
						"  (6         \\  \\    ",
						"    (.      --' |    ",
						"      \\  \\____/ /    ",
						"       \\._____./     ",
						"                     ",
						"       > PEx <       ",
					},
				};

				for (int i = 0; i < scenes.GetLength(0); i++)
				{
					Console.Clear();
					Console.SetCursorPosition(0, height / 2 - (scenes.GetLength(1) / 2));
					for (int j = 0; j < scenes.GetLength(1); j++)
					{
						Console.WriteLine(scenes[i, j].PadLeft(width / 2 + theUltimateAnswerToLifeTheUniverseAndEverything / 4));
					}

					if ((i > 1 && i < 8) || i == 11 || i == 15) { Thread.Sleep(200); }
					else if (i == 13) { Thread.Sleep(3600); }
					else { Thread.Sleep(1800); }
				}
				egg = false;
			}
		}
	}
}