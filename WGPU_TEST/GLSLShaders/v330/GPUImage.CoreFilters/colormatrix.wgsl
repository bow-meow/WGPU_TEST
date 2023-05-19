struct VertexOutput{
	@builtin(position) clip_position: vec4<f32>,
	@location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	color_matrix: mat4x4<f32>
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32> {
    let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	let col: vec4<f32> = vec4<f32>(src.xyz, 1.0);
	let col2: vec4<f32> = col * data.color_matrix;
	return vec4<f32>(col2.xyz, src.w);
}