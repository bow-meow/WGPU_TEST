#version 430 core

uniform sampler2D input_texture;

in VertexData{
	vec2 uv;
	vec2 uv2;
} vert_out;

out vec4 frag_colour;

vec3 RGB2HSV(float r, float g, float b)
{
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

void main()
{
	vec4 src = texture(input_texture, vert_out.uv);
    vec3 hsv = RGB2HSV(src.r, src.g, src.b);
	frag_colour = vec4(hsv.zzz, 1.0);
}