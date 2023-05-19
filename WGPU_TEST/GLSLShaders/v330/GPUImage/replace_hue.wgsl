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

fn HSV2RGB(hsv: vec3<f32>) -> vec3<f32>
{
    var rgb = vec3<f32>(hsv.z, hsv.z, hsv.z);
    if (hsv.y != 0.0) {
       let var_h: f32 = hsv.x * 6;
       let var_i: f32 = floor(var_h);
       let var_1: f32 = hsv.z * (1.0 - hsv.y);
       let var_2: f32 = hsv.z * (1.0 - hsv.y * (var_h - var_i));
       let var_3: f32 = hsv.z * (1.0 - hsv.y * (1 - (var_h - var_i)));
       if      (var_i == 0.0) { rgb = vec3(hsv.z, var_3, var_1); }
       else if (var_i == 1.0) { rgb = vec3(var_2, hsv.z, var_1); }
       else if (var_i == 2.0) { rgb = vec3(var_1, hsv.z, var_3); }
       else if (var_i == 3.0) { rgb = vec3(var_1, var_2, hsv.z); }
       else if (var_i == 4.0) { rgb = vec3(var_3, var_1, hsv.z); }
       else					  { rgb = vec3(hsv.z, var_1, var_2); }
   }
   return (rgb);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	var hsv: vec3<f32> = RGB2HSV(src);

	hsv.x /= 360.0;
    hsv.y = 1.0;
    hsv.z = 1.0;
    let rgb: vec3<f32> = HSV2RGB(hsv);
	return vec4<f32>(rgb, 1.0);
}