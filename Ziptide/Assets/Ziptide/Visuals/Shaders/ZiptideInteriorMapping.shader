// HARDWIRING 1.2 / BUILDING_INTERIORS Tier A — interior mapping: a fake lit room behind every window
// with ZERO interior geometry (the verified technique in VR_TECHNIQUE_RESEARCH §2). The fragment
// shader raycasts the object-space view ray into a virtual one-room box behind the pane and shades
// whichever wall it hits, so the room parallaxes correctly as the player moves — the whole city
// reads as inhabited for the cost of one thin quad per window.
//
// Ziptide's first custom shader — kept mobile-simple on purpose (Meta: shader ALU is the #1 GPU cost
// on Quest): no textures, procedural room tints from the per-object world position, one branchless
// plane pick. Full single-pass-instanced stereo macros so both Quest eyes converge correctly.
Shader "Ziptide/InteriorMapping"
{
    Properties
    {
        _RoomTint    ("Room Tint", Color) = (0.55, 0.5, 0.42, 1)
        _LightColor  ("Lit Back Wall", Color) = (1.0, 0.85, 0.6, 1)
        _RoomDepth   ("Room Depth (object units)", Range(0.2, 3)) = 1.0
        _DarkChance  ("Dark Room Chance", Range(0, 1)) = 0.35
        _GlassTint   ("Glass Tint", Color) = (0.75, 0.85, 0.9, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        Pass
        {
            Name "Forward"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _RoomTint;
                half4 _LightColor;
                float _RoomDepth;
                float _DarkChance;
                half4 _GlassTint;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionOS  : TEXCOORD0;
                float3 viewDirOS   : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionOS = IN.positionOS.xyz;
                float3 camOS = TransformWorldToObject(GetCameraPositionWS());
                OUT.viewDirOS = IN.positionOS.xyz - camOS; // from camera to surface, object space
                return OUT;
            }

            // Cheap deterministic hash of the per-object world position → stable room personality.
            float Hash(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898, 78.233, 37.719))) * 43758.5453);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                // One virtual room per pane. Entry point on the window plane in [0,1]².
                float2 room = saturate(IN.positionOS.xy + 0.5);

                // March INTO the pane along local z regardless of which face the camera sees.
                float3 d = normalize(IN.viewDirOS);
                float zSign = d.z >= 0.0 ? 1.0 : -1.0;
                d.z = max(abs(d.z), 1e-4) ;             // into the room, never parallel
                float3 dir = float3(d.x * zSign, d.y, d.z); // mirror x when seen from the back face

                // Distances to the room box planes [0,1]x[0,1]x[0,_RoomDepth].
                float tx = ((dir.x >= 0.0 ? 1.0 : 0.0) - room.x) / (abs(dir.x) < 1e-4 ? (dir.x >= 0.0 ? 1e-4 : -1e-4) : dir.x);
                float ty = ((dir.y >= 0.0 ? 1.0 : 0.0) - room.y) / (abs(dir.y) < 1e-4 ? (dir.y >= 0.0 ? 1e-4 : -1e-4) : dir.y);
                float tz = _RoomDepth / dir.z;
                float t = min(min(tx, ty), tz);

                // Per-building room personality from the object's world anchor.
                float3 anchor = float3(UNITY_MATRIX_M._m03, UNITY_MATRIX_M._m13, UNITY_MATRIX_M._m23);
                float h = Hash(floor(anchor * 4.0));
                half3 tint = _RoomTint.rgb * (0.7 + 0.6 * h);
                half lit = h > _DarkChance ? 1.0h : 0.12h;   // some rooms are dark — a living skyline

                // Shade by which plane the ray hit.
                half3 col;
                if (t == tz)            col = _LightColor.rgb * tint * 1.15h; // lit back wall
                else if (t == ty)
                    col = dir.y > 0.0 ? tint * 0.85h                          // ceiling
                                      : tint * 0.35h;                        // floor
                else                    col = tint * 0.55h;                   // side walls

                col *= lit;
                // Glass film: a whisper of sky tint so panes never read as holes.
                col = lerp(col, _GlassTint.rgb, 0.08h);
                return half4(col, 1.0h);
            }
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Unlit"
}
