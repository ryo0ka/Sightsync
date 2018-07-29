// http://www.gameartisans.org/forums/threads/14601-Backface-Culling-in-Unity-Engine
//   Source of base code.

// https://docs.unity3d.com/Manual/
//   Source of noob comments.

Shader "Custom/Route Shader" {
    // Parameters to be set in inspector. 
    Properties {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Emission("Emission", Color) = (0, 0, 0, 0)
    }
    
    // Each shader in Unity consists of a list of subshaders. 
    // When Unity has to display a mesh, it will find the shader to use,
    // and pick the first subshader that runs on the user’s graphics card.
    SubShader {
        // Legacy Lighting. Defines the material properties of the object.
        // We use the material in many passes by defining them in the subshader.
        // Anything defined here becomes default values for all contained passes.
        // https://docs.unity3d.com/Manual/SL-Material.html
        Material {
            Diffuse[_Color]

            // Color from ambient light (set in the Lighting Window).
            // Typically you want to keep the Diffuse and Ambient colors the same.
            Ambient[_Color]

            // The color of the object when it is not hit by any light.
            Emission[_Emission]

            // Specular[...]
            // Shininess[...]
        }

        Lighting Off

        // The Pass block causes the geometry of a GameObject to be rendered once.
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            // Legacy Texture Combiners.
            // Must be placed at the end of a Pass.
            SetTexture[_MainTex] {
                // Primary is the color from the lighting calculation
                // or the vertex color if it is bound.
                Combine Primary * Texture
            }
        }
    }
    
    // After all Subshaders a Fallback can be defined.
    // It basically says:
    // “if none of subshaders can run on this hardware,
    //  try using the ones from another shader”.
    FallBack "Diffuse"
}

