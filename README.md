# Labyrinth generation script for Unity
This is a script that generates a labyrinth of a specified size. There are two files: Labyrinth.cs with the Labyrinth class that stores the labyrinth data and generation function and LabyrinthGeneration.cs with the Unity script that builds the labyrinth from the supplied wall and terrain objects. It will resize the terrain and build walls based on generated labyrinth.
## Usage
If you want to use it put both files into your Assets. Then you just add the script (LabyrinthGeneration.cs) as a component. You will need to supply the script with a terrain object (**terrainObject**) and a wall object (**wallPrefab**).
You can change the size of the labyrinth by changing the labyrinthSize variable. The script works with all rectangular sizes and should work for all types of walls but I have not yet tested this extensively.

