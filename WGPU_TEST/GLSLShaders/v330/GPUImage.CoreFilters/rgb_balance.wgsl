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

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	var src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);

	src.r += data.red   * 2.0; 
	src.g += data.green * 2.0;
	src.b += data.blue  * 2.0;
	return clamp(src, vec4<f32>(0.0), vec4<f32>(1.0)); // needs to clamp against the same type
}