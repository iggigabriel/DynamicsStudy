# Dynamics Study

Exploring the math explained in this video https://www.youtube.com/watch?v=KPoeNZZ6H4s

Code was not written for general purpose, just as an experiment of few use cases useful for me. 

Some notes why its not great in its current state:
- Demo uses fixed timestep based simulation, so i precalculate the F/D/R parameters at edit time, changing timestep would require recalculation and reserialization of all assets, and for dynamic timesteps FDR should be recalculated every frame
- I serialize F/D/R parameters in both the Dynamics State and Curve fields to make it more convenient to use, but that means data desync is possible, for general purpose use i would change this to single-source-of-truth architecture
- Graph Editor is currently made specifically for this type of curve, but it would be better if it worked with any object that implements specific interface

Made with Unity 2021.3.15f

## Features

### Graph Editor

https://user-images.githubusercontent.com/1765354/208084449-1f5afd39-bfc0-415a-85c4-b15e871ab80f.mp4



### Playground Sample

https://user-images.githubusercontent.com/1765354/208079095-91f09c4a-acf2-468e-a65a-f52c0face000.mp4




### Compatibility with Burst Compiler (100000 objects)

![instancing](https://user-images.githubusercontent.com/1765354/208075441-2bfd4da4-a06b-4c00-b34f-bdc8d2004523.png)
