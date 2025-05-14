// 1211102879 KOH JIA JIE

//-------------------------------------- Acknowledgement -----------------------------------------------------------

// Assets are get from the following website:
// Red, Blue, Purple - https://www.deviantart.com/darksonicgamer/art/Gojo-Satoru-Maskless-Sprite-Sheet-914004433
// red_blue sfx - https://pixabay.com/sound-effects/bfg-laser-89662/
// purple sfx - https://www.myinstants.com/en/search/?name=gojo
// Sticky Missile, Mine - https://anim86.itch.io/space-shooter-ship-constructor
// Explosion - https://xthekendrick.itch.io/2d-explosions-pixel-art-pack-3
// Sticky Missile sfx - https://pixabay.com/sound-effects/cannon-shot-6153/
// Explosion sfx - https://pixabay.com/sound-effects/small-explosion-103779/
//               - https://pixabay.com/sound-effects/explosion-107629/
//               - https://pixabay.com/sound-effects/laser-gun-72558/

// Tool Used:
// Sprite cutter - https://ezgif.com/sprite-cutter
// Background Remover - https://www.remove.bg/upload
// Image Resizer - https://imageresizer.com/
// MP3 Cutter - https://audiotrimmer.com/
// MP3 to WAV Converter - https://cloudconvert.com/mp3-to-wav

// Help From Others:
// ChatGPT - To understand the error codes and messages popout during the runtime or the process of coding
//         - To rewind the syntax that I have forgoten 

//------------------------------------ End Of Acknowledgement ------------------------------------------------------
// ----------------------------------------- Controls --------------------------------------------------------------

// Sticky Missile will be fired automatically for every 5 seconds
// Press Q to fire Blue (10 seconds cooldown)
// Press E to fire Red (10 seconds cooldown)
// Press R to fire Purple (When both Red and Blue are ready)
// Right click on mouse to fire Red and Blue that move around spaceship (When both Red and Blue are ready)
// Ultimate - Merge both Red and Blue by shooting the Red towards the Blue (Press Q -> E)

// -------------------------------------- End Of Controls ----------------------------------------------------------

// using var game = new Lab06.Game1();
using var game = new GAlgoT2430.Engine.GameEngine("Shooter Game", 1024, 600);
game.AddScene("MainScene", new Lab06.MainScene());
game.Run();
