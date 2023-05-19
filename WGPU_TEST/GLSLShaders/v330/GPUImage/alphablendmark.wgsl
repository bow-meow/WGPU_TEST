struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

@group(0) @binding(0) var input_tex: texture_2d<f32>;
@group(0) @binding(1) var input_sampler: sampler;

@group(1) @binding(0) var<uniform> data: DataUniform;

@group(2) @binding(0) var bg_tex: texture_2d<f32>;

struct DataUniform{
	texture_aspect: f32,
	texture_scale: vec2<f32>,
	t: f32
};

fn fs_main(in: VertexOutput) -> @location(0) vec4<f32>
{
	let src: vec4<f32> = textureSample(input_tex, input_sampler, in.tex_coords);
	var uv = in.tex_coords - vec2<f32>(0.5, 0.5);
	let new_x: f32 = uv.x * cos(data.t) - uv.y * sin(data.t);
    let new_y: f32 = uv.x * sin(data.t) + uv.y * cos(data.t);
	uv = vec2<f32>(new_x, new_y);
	uv += vec2<f32>(0.5, 0.5);
	uv.y *= data.texture_aspect;
	uv /= data.texture_scale.xy;

	let dst: vec4<f32> = textureSample(bg_tex, input_sampler, uv * 10.0);

	return vec4<f32>(mix(src.rgb, dst.rgb, dst.a * 0.5), max(src.a, dst.a));
}