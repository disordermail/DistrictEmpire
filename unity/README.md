# District Empire Unity Vertical Slice

Unity 6 LTS portrait, local-only vertical slice. Open `unity/` in Unity Hub, create an empty portrait scene, add a `UIDocument` and the `DistrictEmpireBootstrap` component to one GameObject. Assign a Panel Settings asset to the UIDocument, then make the scene the startup scene.

Architecture: `Domain` holds serializable game state, `Application` owns use cases and simulated clock, `Infrastructure` persists JSON locally, and `Presentation` is an imperative UI Toolkit mobile shell.

The accelerated local clock maps one real second to one simulated minute. Notary transfer takes 12 simulated minutes, listings generate local applicants after 8 simulated minutes, and state is saved to `Application.persistentDataPath`.
