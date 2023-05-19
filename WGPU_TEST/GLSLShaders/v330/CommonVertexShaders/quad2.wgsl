@group(0) @binding(0) var<uniform> data: DataUniform;

struct DataUniform{
    bg_tile_offset: vec2<f32>,
    bg_tile_scale: vec2<f32>
}

struct VertexInput {
    @location(0) position: vec3<f32>,
    @location(1) tex_coords: vec2<f32>,
};

struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) tex_coords: vec4<f32>,
};

fn vs_main(model: VertexInput) -> VertexOutput{
	var out: VertexOutput;
    let sgn: vec2<f32> = sign(position.xy);
    
	out.tex_coords = vec4<f32>(model.tex_coords, data.bg_tile_offset + (uv * data.bg_tile_scale));
    out.position = vec4<f32>(sgn, 0.0, 1.0);

    return out;
}