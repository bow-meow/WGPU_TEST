#version 430 core

uniform sampler2D input_texture;

uniform float keyHue;
uniform float spillEdgeDistance;
uniform float spillEdgeFeather;
uniform vec3 startDists;
uniform vec3 endDists;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec3 RGB2HSV(vec3 color)
{
	float r = color.r;
	float g = color.g;
	float b = color.b;
	float themin,themax,delta;
	vec3 c2;

	themin = min(r,min(g,b));
	themax = max(r,max(g,b));
	delta = themax - themin;
	c2.z = themax;			
	c2.y = 0.0f;			
	if (themax > 0.0f)
		c2.y = delta / themax;
	c2.x = 0.0f;				
	if (delta > 0.0f)
	{
		if (themax == r && themax != g)
			c2.x += (g - b) / delta;
		if (themax == g && themax != b)
			c2.x += (2.0f + (b - r) / delta);
		if (themax == b && themax != r)
			c2.x += (4.0f + (r - g) / delta);
		c2.x *= 60.0f;
	}
	return c2;
}

float CircularDistance(float a, float b, float range)
{
	return min(abs(a-b), abs(abs(a-b) - range));
}

float GetHueDistanceFast(float hue1, float hue2)
{
	return CircularDistance(hue1, hue2, 360.0)/180.0;
}

vec4 SpillSuppression(vec4 col, vec3 hsv, float hueDist)
{			
	hueDist = clamp((hueDist - spillEdgeDistance) / (spillEdgeFeather), 0, 1);

	float dp = dot(col.rgb, vec3(0.299, 0.587, 0.114));
	vec3 targetCol = vec3(dp, dp, dp);

	vec3 final = mix(targetCol, col.rgb, hueDist);
	
	return vec4(final, col.a);
}

vec4 ChromaKey(vec4 col, vec3 hsv, float hueDist)
{
	vec3 values = vec3(hueDist, hsv.y, hsv.z);
	vec3 a = smoothstep(startDists, endDists, values);

	float alpha = (length(vec3(a.x, 1.0-a.y, 1.0-a.z)));

	return vec4(col.rgb, alpha);
}


void main()
{
	vec4 original = texture(input_texture, vert_out.uv);
	vec3 hsv = RGB2HSV(original.rgb);
	float hueDist = GetHueDistanceFast(keyHue, hsv.x);

	frag_colour = SpillSuppression(ChromaKey(original, hsv, hueDist), hsv, hueDist);
}