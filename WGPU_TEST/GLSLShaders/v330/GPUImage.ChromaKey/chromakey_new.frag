#version 330 core

uniform sampler2D input_texture;
uniform vec3 keyHSV;
uniform float hueDistance;
uniform float satDistance;
uniform float valDistance;
uniform float hueFeather;
uniform float satFeather;
uniform float valFeather;
uniform float hueFeather2;
uniform bool premultiply;

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

float GetSafeHueDistance(float hue1, float hue2)
{
	hue1 = radians(hue1);
	hue2 = radians(hue2);

	// Circular distance
	vec2 a = vec2(cos(hue1), sin(hue1));
	vec2 b = vec2(cos(hue2), sin(hue2));
	
	return length(a - b) * 0.5;
}

float SignedCircularDistance(float a, float b, float range)
{
	// non-wrapped abs distance
	float d1 = abs(a-b);
		
	// wrapped abs distance	
	float d2 = range - d1;

	float d = min(d1, d2);
	if (d2 < d1)
	{
		d *= sign(a-b);
	}
	else
	{
		d*= sign(b-a);
	}

	return d;
}

float GetSignedHueDistance(float hue1, float hue2)
{
	return SignedCircularDistance(hue1, hue2, 360)/180.0;
}

void main()
{
	vec4 col = texture(input_texture, v_tex);

	if(premultiply)
	{
		col = vec4(col.r/col.a,col.g/col.a,col.b/col.a,col.a);
	}
	vec3 hsv = RGB2HSV(col.rgb);

	float hueDist = GetHueDistance(keyHSV.x, hsv.x);
	float hueDist2 = GetSafeHueDistance(keyHSV.x, hsv.x);
	float hueDist3 = GetSignedHueDistance(keyHSV.x, hsv.x);


	vec3 values = vec3(abs(hueDist3), hsv.y, hsv.z);
	vec3 startDists = vec3(hueDistance, satDistance, valDistance);
	vec3 feathers = vec3(hueFeather, satFeather, valFeather);
	if (hueDist3 < 0.0)
	{
		feathers.x = hueFeather2;
	}
	vec3 endDists = startDists + feathers;
	vec3 a = smoothstep(startDists, endDists, values);
	

	float alpha = clamp(length(vec3(a.x, 1.0-a.y, 1.0-a.z)), 0, 1);

	col.a = min(alpha, col.a);

	if(premultiply)
	{
		frag_colour = vec4(col.r*alpha,col.g*alpha,col.b*alpha,alpha);
	}
	else
	{
		frag_colour = col;
	}
}          