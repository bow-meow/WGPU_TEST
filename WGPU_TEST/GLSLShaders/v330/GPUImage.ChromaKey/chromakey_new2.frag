#version 430 core

uniform sampler2D input_texture;
uniform vec3 keyHSV;
uniform float hueDistance;
uniform float satDistance;
uniform float valDistance;
uniform float hueFeather;
uniform float satFeather;
uniform float valFeather;
uniform float hueFeather2;

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
	return min(abs(a-b), range - abs(a-b));
}

float GetHueDistance(float hue1, float hue2)
{
	return CircularDistance(hue1, hue2, 360)/180.0;
}

void main()
{
	vec4 col = texture(input_texture, vert_out.uv);

	vec3 hsv = RGB2HSV(col.rgb);

	float hueDist = GetHueDistance(keyHSV.x, hsv.x);

	vec3 values = vec3(hueDist, hsv.y, hsv.z);
	vec3 startDists = vec3(hueDistance, satDistance, valDistance);
	vec3 feathers = vec3(hueFeather, satFeather, valFeather);
	vec3 endDists = startDists + feathers;
	vec3 a = smoothstep(startDists, endDists, values);
	
	float alpha = clamp(length(vec3(a.x, 1.0-a.y, 1.0-a.z)), 0, 1);

    col.a = min(alpha, col.a);

    frag_colour = col;
}