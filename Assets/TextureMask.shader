Shader "Custom/TextureMaskShader" {
	Properties
	{
		//_SomeText("Scrub Layer Name", Int) = 5
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_CutawayThreshold("Cutaway Threshold", Range (0, 1)) = 1
	}
	SubShader
	{
		/*Tags
		{
			"Queue" = "Transparent+2"
			"RenderType" = "Transparent"
		}
		LOD 100*/
		BindChannels
		{
			Bind "vertex", vertex
			Bind "color", color
		}

		Pass
		{

			/*Color[_Color]
			SetTexture[_MainTex] 
			{
				ConstantColor [_Color]
				Combine constant Lerp(constant) texture
			}*/



			//CGPROGRAM
			//#pragma vertex vert
			//#pragma fragment frag
			//		// make fog work
			//#pragma multi_compile_fog

			//#include "UnityCG.cginc"

			//struct appdata
			//{
			//	float4 vertex : POSITION;
			//	float2 uv : TEXCOORD0;
			//};

			//struct v2f
			//{
			//	float2 uv : TEXCOORD0;
			//	UNITY_FOG_COORDS(1)
			//		float4 vertex : SV_POSITION;
			//};

			//sampler2D _MainTex;
			//fixed4 _Color;
			//float4 _MainTex_ST;

			//v2f vert(appdata v)
			//{
			//	v2f o;
			//	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			//	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			//	UNITY_TRANSFER_FOG(o,o.vertex);
			//	return o;
			//}

			//fixed4 frag(v2f i) : SV_Target
			//{
			//	// sample the texture
			//	fixed4 col = tex2D(_MainTex, i.uv);
			//	// apply fog
			//	UNITY_APPLY_FOG(i.fogCoord, col);
			//	return col;
			//}
			//ENDCG

			CGPROGRAM

				#pragma vertex vert  
				#pragma fragment frag 

				struct vertexInput {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				struct vertexOutput {
					float4 pos : SV_POSITION;
					//float4 posInObjectCoords : TEXCOORD0;
					float2 uv : TEXCOORD0; // texture coordinate
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
					//output.posInObjectCoords = input.vertex;
					output.uv = input.uv;

					return output;
				}

				sampler2D _MainTex;
				float _CutawayThreshold;

				fixed4 frag(vertexOutput input) : COLOR
				{
					if (input.uv.y > _CutawayThreshold || input.uv.x > _CutawayThreshold)
					{
						discard; // drop the fragment if y coordinate > 0
					}
					//return float4(0.0, 1.0, 0.0, 1.0); // green
					fixed4 col = tex2D(_MainTex, input.uv);
						
					return col;
				}

			ENDCG
		}
	}
}
