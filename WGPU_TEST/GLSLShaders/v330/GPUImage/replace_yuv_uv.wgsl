struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

fn RGB2YUV(rgb: vec4<f32>) -> vec4<f32>
{
	let coeffs= mat4x4<f32>(
			 0.299, 0.587, 0.114, 0.000,
			-0.147,-0.289, 0.436, 0.000,
			 0.615,-0.515,-0.100, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
		
	return coeffs * rgb;
}

fn YUV2RGB(yuv: vec4<f32>) -> vec4<f32>
{
	let coeffs= mat4x4(
			 1.000, 0.000, 1.140, 0.000,
			 1.000,-0.395,-0.581, 0.000,
			 1.000, 2.032, 0.000, 0.000,
			 0.000, 0.000, 0.000, 0.000
		);
	
	return coeffs * yuv;
}

@fragment
fn fs_main(in: VertexOutput)
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
    var yuv: vec4<f32> = RGB2YUV(src);
	yuv.x = 0.5;
	let rgb: vec4<f32> = (YUV2RGB(yuv));
	return vec4<f32>(rgb.xyz, 1.0);
}