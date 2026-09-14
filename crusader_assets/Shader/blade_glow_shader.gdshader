shader_type spatial;
render_mode blend_mix, unshaded, cull_disabled, depth_draw_never;

uniform float global_intensity : hint_range(0.0, 6.0) = 1.0;

// ============ 核心亮带 ============
uniform vec4  core_color : source_color = vec4(1.0, 0.97, 0.88, 1.0);
uniform float core_intensity : hint_range(0.0, 10.0) = 3.0;
uniform float core_width : hint_range(0.0, 1.0) = 0.0;
uniform float core_power : hint_range(1.0, 8.0) = 3.0;

// ============ 光晕 ============
uniform vec4  halo_color : source_color = vec4(1.0, 0.9, 0.6, 1.0);
uniform float halo_intensity : hint_range(0.0, 10.0) = 3.0;
uniform float halo_width : hint_range(0.1, 1.0) = 1.0;
uniform float halo_power : hint_range(0.0, 4.0) = 0.3;

// ============ 两端淡出 ============
uniform float end_fade : hint_range(0.01, 1.0) = 0.9;

// ============ 透明度控制 ============
// 让多亮以下的部分完全透明（消除边缘黑线）
uniform float alpha_cutoff : hint_range(0.0, 1.0) = 0.3;

// 透明度增益：越高，整条光越实
uniform float alpha_gain : hint_range(0.0, 10.0) = 3.0;

void fragment() {
    float d = abs(UV.y - 0.5) * 2.0;

    float core = pow(max(0.0, 1.0 - d / max(core_width, 1e-4)), core_power);
    float halo = pow(max(0.0, 1.0 - d / max(halo_width, 1e-4)), halo_power);

    float edge_guard = smoothstep(0.0, 0.25, min(UV.y, 1.0 - UV.y) * 2.0);
    float end_mask = smoothstep(0.0, end_fade, UV.x)
                   * (1.0 - smoothstep(1.0 - end_fade, 1.0, UV.x));

    vec3 col = core_color.rgb * core * core_intensity
             + halo_color.rgb * halo * halo_intensity;

    col *= edge_guard * end_mask * global_intensity;

    // ---- 亮度决定 alpha：暗处透明，亮处不透明 ----
    float lum = max(max(col.r, col.g), col.b);
    float alpha = clamp((lum - alpha_cutoff) * alpha_gain, 0.0, 1.0);

    ALBEDO = col;
    ALPHA = alpha;
}