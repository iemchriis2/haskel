This directory contains the scenes that will be in builds. (no sub directory scenes are included)

EntryPoint is the first scene to load and stays loaded the entire time.

Stage0, Stage1, and Stage2 are additively loaded and removed as needed.
	As such, these scenes should not have cameras or directional lights.
		Point lights that are part of scenery are okay.
