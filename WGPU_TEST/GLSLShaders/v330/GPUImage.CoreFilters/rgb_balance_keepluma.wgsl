struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	red: f32,
	green: f32,
	blue: f32
};

fn rgb2yuv(rgb: vec4<f32>) -> vec4<f32>
{
	let coeffs = mat4x4<f32>(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

fn yuv2rgb(yuv: vec4<f32>) -> vec4<f32>
{
	let coeffs = mat4x4<f32>(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	var src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	let yuv: vec4<f32> = rgb2yuv(src);

	src.r += data.red * 2.0;
	src.g += data.green * 2.0;
	src.b += data.blue * 2.0;
	src = clamp(src, vec4<f32>(0.0), vec4<f32>(1.0)); // needs to clamp to same type

	let yuv2: vec4<f32> = rgb2yuv(src);

	// WGSL does not support swizzles (vec4.xyz = vec4.xyz).. need to do it the long way
	let res = yuv2rgb(vec4(yuv.x, yuv2.yz, 1.0)).xyz;
	src.x = res.x;
	src.y = res.y;
	src.z = res.z;

	return src;
}