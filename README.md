# Project1

Brief description of the project's implementation

## Procedural Terrain Generation in Unity

**Implementation Steps**:

1. Create a Vector3 array with a size of $$(divisions + 1)^2$$
2. Randomise the four corners of the terrain such that we can proceed to do the diamond step.
3. Fill in the inside part of the terrain with diamond-square steps
   - A random variable `gradient` were add such that the maximum height were different each iteration of the algorithm.
   - While on each iteration, also assign the colour to each vertex based on its height in order to increase the calculation efficiency.
4. Add the mesh collider to the generated mesh.
5. Add in the water which also using the Diamond-Square algorithm to generate
   - By using $$sin()$$ function the wave-like movement can be created

## Camera

**Implementation Steps**:

1. Initialised the starting position.
2. Detect the movement control automatically by adding `Input.GetAxis` which were already defined in the Unity default engine.
   - `Vertical` and `Horizontal` for moving forward, backward, left and right.
   - `MouseX` and `MouseY` for camera's yaw and pitch.
3. The Unity engine does not provide the roll part, we have to manually add that into the `InputManager` of the engine, and thus enabling us to use the `Input.GetAxis` with the newly defined set.
4. ​

**NOTE**:

All parameters are customizable through Unity Inspector.