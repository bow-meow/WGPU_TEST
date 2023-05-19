struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
	window_size: vec2<f32>
};

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src = textureSample(input_tex, input_sampler, in.tex_coords);
	var checkerboard = vec4<f32>(0.5, 0.5, 0.5, 1.0);
	if((i32(floor(in.tex_coords.x * data.window_size.x) + floor(in.tex_coords.y * data.window_size.y)) & 1) == 0){
		checkerboard = vec4<f32>(1.0, 1.0, 1.0, 1.0);
	}
 	return vec4<f32>(src.rgb * src.a + (1.0 - src.a) * checkerboard.rgb, 1.0);
}
