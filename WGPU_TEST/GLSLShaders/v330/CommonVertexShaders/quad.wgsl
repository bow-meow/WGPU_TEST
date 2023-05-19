struct VertexInput {
    @location(0) position: vec3<f32>,
    @location(1) tex_coords: vec2<f32>,
};

struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec2<f32>,
};

fn vs_main(model: VertexInput) -> VertexOutput
{
	var out: VertexOutput;
	let sgn: vec2<f32> = sign(model.position.xy);
	out.tex_coords = model.tex_coords;
	out.clip_position = vec4<f32>(sgn, 0.0, 1.0)

	return out;
}