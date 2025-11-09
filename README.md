# Unity-Template

### Overview
A Template / example project that has been set up with the SenseGlove Unity Plugin and relevant VR Plugins. Useful when you're getting started.
There are some simple examples inside the Scenes/Examples_Scene.

This Template was created using Unity 2022.3.62f3, SenseGlove Unity Plugin v2.9.0

More information about the SenseGlove Unity SDK can be found at: https://senseglove.gitlab.io/SenseGloveDocs/

### Scenes

Example scene:
The interactions are made with Unity primitives to give an idea of how to work with the SenseGlove plugin. 
Interactions included are:
    - Simple interactions (grabbing, squeezing, placing inside highlight, breakable)
    - Advanced interactions (drawer, door)
    - Snapping interactions (drill example with trigger)
    - Buttons, switches and dials

Template scene:
A scene with the camera setup and the gloves placed.

Small objects scene:
A scene with a basic and crude example of how it could work if you have a lot of small objects and want to try to pick them up with the SenseGlove hands.

### Teleporting

Inside the Examples_Scene you can teleport around by pointing with your index finger to the spot you want to go.
The pointing works gesture based and can always be adjusted to another gesture.
If a green beam shows up a timer goes off and if the timer is complete you will teleport to the spot on the ground where the green beam hits the floor.
If the red beam shows up then the timer will reset and not start as that point is not allowed to teleport to.
