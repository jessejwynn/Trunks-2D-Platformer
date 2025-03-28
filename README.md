# Readme

TrunksMcDuncan/
├── Assets/            # Contains all game assets (Scenes, Scripts, Sprites, Animations, Prefabs, etc.)
├── Packages/          # Unity Package Manager files
├── ProjectSettings/    # Unity project settings
├── Misc Files/  	#Other files

## Build instructions



### Requirements

- Unity Editor: 2022.346f1
- Operating System: Windows 10/11, macOS, or Linux
- Visual Studio / Code Editor: (e.g., Visual Studio, VS Code) for script editing
- GitHub: Version Control System


### Build steps

1. Clone the project
- git clone https://github.com/jessejwynn/Trunks-2D-Platformer.git

2. Open Project in Unity
- Open Unity Hub.
- Click on the Open Project button.
- Navigate to the folder containing your project (the folder with Assets, Packages, ProjectSettings).
- Click Open and let Unity load the project (this may take a few minutes).

3. Installing Dependencies
- Check the Package Manager (Window > Package Manager) and install necessary packages
- Ensure all scripts are properly imported and no missing references appear in the Console window


### Test steps

- Navigate to Assets > Scenes
- Double-click/select 'Cutscene'
- Click the Play button at the top of the Unity Editor
- The game should start with the cutscenes (press any key to continue)
- Once the cutscenes end, the tutorial will start. Press any key to continue (tutorial dialogues)
- Once you reach the flag at the end of the level, select the levels. 
- Ensure that you press the key 'P' at the start of each level (debugging key)

Controls:
- Movement: WASD or Arrow Keys (WASD recommended)
- Jump/Double Jump: Spacebar
- Run: Left Shift
- Dash: Enter 


Issues/troubleshooting:
- If errors occur, check the Console window (Window > General > Console) for messages.
- Make sure Unity version matches the one specified in the requirements.
- Ensure all necessary packages are installed in the Package Manager.
- If the game doesn't run correctly, verify the PlayerController.cs script and its settings in the Inspector.
