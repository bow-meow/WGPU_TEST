#version 330 core

uniform sampler2D input_texture;

uniform vec3 keyHSV;
uniform float edgeDistance;
uniform float edgeFeather;

//in VertexData {
//	vec2 uv;
//	vec2 uv2;
//} vert_out;

in vec2 v_tex;
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
	c2.z = themax;			// value
	c2.y = 0.0f;				// saturation
	if (themax > 0.0f)
		c2.y = delta / themax;
	c2.x = 0.0f;				// hue
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
	return min(abs(a-b), range - abs(a-b));
}

float GetHueDistance(float hue1, float hue2)
{
	return CircularDistance(hue1, hue2, 360)/180.0;
}

void main()
{
	vec4 col = texture(input_texture, v_tex);
	if (col.a > 0.0)
	{
		vec3 sourceHSV = RGB2HSV(col.rgb);

		float hueDistance = GetHueDistance(sourceHSV.x, keyHSV.x);

		// create mask value, with feathering
		hueDistance = clamp((hueDistance - edgeDistance) / (edgeFeather), 0, 1);

		float dp = dot(col.rgb, vec3(0.299, 0.587, 0.114));
		vec3 targetCol = vec3(dp, dp, dp);
		col.rgb = mix(targetCol, col.rgb, hueDistance);
	}
 		frag_colour = col;
}