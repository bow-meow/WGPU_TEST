struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

fn RGB2HSV(color: vec3<f32>) -> vec3<f32>
{
	let r: f32 = color.r;
	let g: f32 = color.g;
	let b: f32 = color.b;
	
	let the_min = min(r, min(g, b));
	let the_max = max(r, max(g, b));
	let delta = the_max - the_min;
	
	var c2: vec3<f32>;
	
	c2.z = the_max;				// value
	c2.y = 0.0;					// saturation
	if (the_max > 0.0){
		c2.y = delta / the_max;
	}
	c2.x = 0.0;					// hue
	if (delta > 0.0)
	{
		if (the_max == r && the_max != g){
			c2.x += (g - b) / delta;
		}
		if (the_max == g && the_max != b){
			c2.x += (2.0 + (b - r) / delta);
		}
		if (the_max == b && the_max != r){
			c2.x += (4.0 + (r - g) / delta);
		}
		c2.x *= 60.0;
	}
	return c2;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
    let hsv: vec3<f32> = RGB2HSV(src);
	return vec4<f32>(hsv.zzz, 1.0);
}