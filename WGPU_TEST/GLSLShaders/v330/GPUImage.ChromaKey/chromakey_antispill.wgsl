struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	key_hsv: vec3<f32>,
	edge_distance: f32,
	edge_feather: f32
};

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

fn CircularDistance(a: f32, b: f32, range: f32) -> f32
{
	return min(abs(a - b), range - abs(a - b));
}

fn GetHueDistance(hue1: f32, hue2: f32) -> f32
{
	return CircularDistance(hue1, hue2, 360.0) / 180.0;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	var src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	if (src.a > 0.0)
	{
		let source_hsv: vec3<f32> = RGB2HSV(src.rgb);

		var hue_distance: f32 = GetHueDistance(source_hsv.x, data.key_hsv.x);

		// create mask value, with feathering
		hue_distance = clamp((hue_distance - data.edge_distance) / (data.edge_feather), 0.0, 1.0);

		let dp: f32 = dot(src.rgb, vec3<f32>(0.299, 0.587, 0.114));
		let target_col = vec3<f32>(dp, dp, dp);
		let mixed_target_src: vec3<f32> = mix(target_col, src.rgb, hue_distance);

		src = vec4<f32>(mixed_target_src, src.a);
	}
 	return src;
}