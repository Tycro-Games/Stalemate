## Stalemate
A novel strategy game where you play as both sides. For now the development has been discontinued

## Demo in Itch.io
https://github.com/user-attachments/assets/5e14f5e1-a604-4560-8768-2d09ab5b4cd7

[🎮 Play on Itch.io ](https://jasjasdev.itch.io/stalemate)

## Features
- Configurable gameplay loop
- AI that adapts to designer made constraints
- Custom Scriptable Objects for making new unit types
- Serialization system for the unit types
- Audio integration with FMOD

### Gameplay Loop
The design of the game has suffered a few iteration, therefore the code assumes a linear structure to each phase of the game. Broadly speaking, the gameplay loop is:

- Player selects side to play as
- Show fog of war for the opposing side
- Player makes new placements
- Units advance their turn which is: attack, move, boost(certain units can retrigger the first two actions)
- Repeat until final turn is reached, or one side looses
When one side looses a menu pops-up that allows the player to change a unit from the losing side.

Below is the `Phase System` that is used to configure each of the events mentioned above via events.
<img width="508" height="894" alt="phase" src="https://github.com/user-attachments/assets/510e76fc-49b8-48a8-a5c8-938aaf0a0d2d" />

### AI system
Stalemate is based on chess and it is using **backtracking** to generate all possible permutations of the 5 units in the 4 squares that are available at a time. In the project there is a scene called **Backtracking** that showcases how the AI rates a board. In the video below you can see how the test scene is used to generate permutation using a max cost. These are sorted based on a developer defined rating. Blue side is configured to have a smaller rating if there are fewer units placed, therefore, the highest rating boards have as many units as possible.


https://github.com/user-attachments/assets/4d294881-11e9-4b09-950e-336594f07d4c


The rating is defined using a ScriptableObject offline:

<img width="477" height="221" alt="image" src="https://github.com/user-attachments/assets/32afb77e-f88b-46a4-bccf-46edd666b18e" />




## Making new units
Each unit is represented with ScriptableObjects. It is as simple as providing the gameplay information and adding a grayscale sprite. The pipeline uses a shader that colors the each of the sprite with a different color palette.
<img width="541" height="448" alt="Screenshot 2025-11-04 131333" src="https://github.com/user-attachments/assets/62c0b3a8-021a-400e-b29b-5b1e5618e5a0" />




## Credits
Code - [Bogdan Mocanu](https://github.com/OneBogdan01)

Everying else - [Jasmine de Jong ](https://jasjasdev.itch.io/)
