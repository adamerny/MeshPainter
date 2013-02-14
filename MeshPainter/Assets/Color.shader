Shader "VertexColor" 
{

Category {
	Tags {"IgnoreProjector"="True" "RenderType"="Opaque" }
	
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels 
	{
		Bind "Color", color
		Bind "Vertex", vertex
	}
	
	SubShader 
	{
		Pass 
		{
		}
	}
}
}