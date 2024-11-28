There are a few steps to take to make the teleport through gesture (pointing) working:

- Place this prefab in the hand you want to teleport with under SGHandRight or SGHandLeft.

- Create and Set the Layer of the ground you want to teleport on.

- Place the Camera rig (XRRig) in the Camera rig location of the script in the editor.
If you leave the camera rig empty you could also use the Event that gives the destination where the teleport beam is pointed at.


If you want an other gesture than pointing, you can change the gesture values in the child of the teleport prefab called TeleportGesture.