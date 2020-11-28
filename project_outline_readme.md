# Project outline for group 4 - Rubberbanding

The stubs for our project can be found under the rubberbanding directories, under the unity directory:

/unity/Assets/RubberBanding
/unity/Assets/Scripts/RubberBanding

We divide the work into 3 main components namely:

- Convex hull related methods, and data structures.
- Range query tree (RQT) related methods, and data structures.
- General game logic, interaction, AI and UI.

Erik will be working on the RQT components. Paul will be working on the convex hull related methods, and Jurrien will work on the general game logic, AI and UI.

The /unity/Assets/Scripts/RubberBanding directory is split up into Controller, Model and UI directories. Below I will detail the source files in each directory:

/Controller:

rbPlayer.cs: Class to accomodate variables related to a player such as a score and an ID.
rbAI.cs: Subclass of the player class which extends the player class with AI-related methods so that we can implement an AI opponent.
rnController: Top-level controller which manages the game.

/Model:

rbConvexHull.cs: Class to accomodate the convex hull data structure together with methods related to convex hull construction and updates.
rbGameManager.cs: Manages game state such as the turns, what points are being removed, score and other game related logic.
rbLevel.cs: Contains info and methods about the current level, and ties in with the UI to draw it.
rbRangeTree: Class to implement the rangeQueryTree.

/UI

rbPoint.cs: Class to accomodate UI logic to draw a point.
rbSegment.cs: Class to accomodate UI logic to draw line segments.

If there are any further questions about our division of work, feel free to message us on discord.