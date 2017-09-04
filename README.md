# Project1

Brief description of the project's implementation

## Procedural Terrain Generation in Unity

**Implementation Steps**:

1. Create a Vector3 array with a size of `(divisions + 1)^2`
2. Randomise the four corners of the terrain such that we can proceed to do the diamond step.
3. Fill in the inside part of the terrain with diamond-square steps
   - A random variable `gradient` were add such that the maximum height were different each iteration of the algorithm.
   - While on each iteration, also assign the colour to each vertex based on its height in order to increase the calculation efficiency.
4. Add the mesh collider to the generated mesh.
5. Add in the water which also using the Diamond-Square algorithm to generate
   - By using `sin()` function the wave-like movement can be created

## Camera

**Implementation Steps**:

1. Initialised the starting position.
2. Detect the movement control automatically by adding `Input.GetAxis` which were already defined in the Unity default engine.
   - `Vertical` and `Horizontal` for moving forward, backward, left and right.
   - `MouseX` and `MouseY` for camera's yaw and pitch.
3. The Unity engine does not provide the roll part, we have to manually add that into the `InputManager` of the engine, and thus enabling us to use the `Input.GetAxis` with the newly defined set.
4. Add a sphere collision into the camera object in the Unity engine.
5. Add a Character Controller into the camera object in the Unity engine.
   - Instead of directly moving the camera by `Transform.Translate` use the controller interface `CharacterController.Move` such that the Unity can automatically prevent the collision from happening.

## Sun & Moon

**Implementation Steps**:

1. Sun and moon can be easily done as one of them is the opposite of the another.
2. In this project we assume that `x-axis` is the East-West axis.
3. Initialise sun at the top of the radius and moon on the other end.
4. Add `Lookat (0, 0)` in the script such that the object will face the origin and thus, generating day and night cycle
5. Modify the colour of the sun and moon
   - Sun being bright orange
   - Moon being dark blue