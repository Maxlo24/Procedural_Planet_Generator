Shader "Custom/Sphere"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _Frequency("Frequency", Range(0, 100)) = 10
        _Amplitude("Amplitude", Range(0.0, 10.0)) = 0.1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadow addshadow vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;

            static const float PI = 3.14159265;

            struct Input
            {
                float2 uv_MainTex;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;

            float _Frequency;
            float _Amplitude;

            float Wave(float x, float z, float t)
            {
                float y = sin(PI * (x + 0.5 * t));

                return y * (.25);
            }

            float MultiWave(float x, float z, float t)
            {
                float y = sin(PI * (x + 0.5 * t));
                y += 0.5 * sin(2 * PI * (z + t));
                y += sin(PI * (x + z + 0.25 * t));

                return y * (.25);
            }

            float Ripple(float x, float z, float t) {
                // Pitagora
                float d = sqrt(x * x + z * z);
                float y = sin(PI * (4 * d - t));

                return y / (1 + 10 * d);
            }

            float TransversalWaves(float3 p, float3 d, float v, float period, float phase)
            {
                float u = float3(d.y, d.z, d.x);
                float numerator = _Time.y - dot(p, (d / v));

                return _Amplitude / 2 * u * sin((numerator / period) + phase);
            }

            float LongitudinalWaves(float p, float omega, float c)
            {
                float x = distance(p, float3(0, 0, 0));

                return _Amplitude / 10 * cos(omega * (_Time.y - (x / c)));
            }

            float3 MultiWave3D(float u, float v, float t)
            {
                float3 p;

                p.x = u;
                p.y = sin(PI * (u + 0.5 * t));
                p.y += 0.5 * sin(2 * PI * (v + t));
                p.y += sin(PI * (u + v + 0.25 * t));
                p.y *= 1 / 2.5;
                p.z = v;

                return p;
            }

            float3 RingTorus(appdata_base v)
            {
                float r1 = 0.75;
                float r2 = 0.25;
                float s = r1 + r2 * cos(PI * v.normal.y);

                v.vertex.x = s * sin(PI * v.normal.z);
                v.vertex.y = r2 * sin(PI * v.normal.y);
                v.vertex.z = s * cos(PI * v.normal.z);

                return v.vertex.xyz;
            }

            float3 FloatingTorus(appdata_base v)
            {
                float r1 = 0.7 + 0.1 * sin(PI * (6 * v.normal.z + 0.5 * _Time.y));
                float r2 = 0.15 + 0.05 * sin(PI * (8 * v.normal.z + 4 * v.normal.y + 2 * _Time.y));
                float s = r1 + r2 * cos(PI * v.normal.y);

                v.vertex.x = s * sin(PI * v.normal.z);
                v.vertex.y = r2 * sin(PI * v.normal.y);
                v.vertex.z = s * cos(PI * v.normal.z);

                return v.vertex.xyz;
            }

            void vert(inout appdata_base v)
            {
                //v.vertex.x += TransversalWaves(v.vertex.xyz, v.normal.xyz, 1, 1, PI);
                //v.vertex.y += TransversalWaves(v.vertex.xyz, v.normal.xyz, 1, 1, PI);
                //v.vertex.z += TransversalWaves(v.vertex.xyz, v.normal.xyz, 1, 1, PI);

                //v.vertex.x += LongitudinalWaves(v.vertex.xyz, PI / 1, 4);
                //v.vertex.y += LongitudinalWaves(v.vertex.xyz, PI / 1, 4);
                //v.vertex.z += LongitudinalWaves(v.vertex.xyz, PI / 1, 4);

                //v.vertex.y += sin(PI * (v.vertex.x + _Time.y)) + pow(sin(2 * PI * (v.vertex.x + _Time.y)), 2);
                //v.vertex.xyz += v.vertex.xyz * sin(v.vertex.x * _Frequency + _Time.y) * _Amplitude/10;

                v.vertex.y *= 0.9 + 0.1 * sin(PI * (6 * v.vertex.x + 4 * v.vertex.y + _Time.y));

                //v.vertex.xyz = RingTorus(v);
                //v.vertex.xyz = FloatingTorus(v);

                //v.vertex.y = Wave(v.vertex.x, v.vertex.z, _Time.y);

                //v.vertex.y = MultiWave(v.vertex.x, v.vertex.z, _Time.y);

                //v.vertex.y = Ripple(v.vertex.x, v.vertex.z, _Time.y);

                //v.vertex.xyz = MultiWave3D(v.vertex.x, v.vertex.z, _Time.y);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}