shader_type canvas_item;
render_mode blend_add, unshaded;

// ============ 布局 ============
// ColorRect 是正方形时填 1.0；宽屏则填 宽/高
uniform float aspect : hint_range(0.1, 10.0) = 1.0;
uniform float global_intensity : hint_range(0.0, 6.0) = 1.0;

// ============ 中心核心 ============
uniform vec4 core_color : source_color = vec4(1.0, 0.97, 0.88, 1.0);
uniform float core_size : hint_range(0.0, 0.8) = 0.05;
uniform float core_power : hint_range(0.5, 16.0) = 3.0;
uniform float core_intensity : hint_range(0.0, 10.0) = 2.5;

// ============ 外圈柔光 ============
uniform vec4 halo_color : source_color = vec4(1.0, 0.69, 0.35, 1.0);
uniform float halo_size : hint_range(0.05, 1.5) = 0.50;
uniform float halo_power : hint_range(0.5, 16.0) = 2.5;
uniform float halo_intensity : hint_range(0.0, 5.0) = 0.8;

// ============ 放射状光束（丁达尔） ============
uniform vec4 ray_color : source_color = vec4(1.0, 0.82, 0.50, 1.0);
uniform int ray_count : hint_range(2, 64) = 14;
uniform float ray_intensity : hint_range(0.0, 5.0) = 0.7;
uniform float ray_length : hint_range(0.05, 1.5) = 0.55;
uniform float ray_power : hint_range(0.5, 12.0) = 2.5;
uniform float ray_sharpness : hint_range(0.0, 1.0) = 0.55;
uniform float ray_speed : hint_range(0.0, 2.0) = 0.06;
uniform float ray_inner_fade : hint_range(0.0, 0.6) = 0.12;

// ============================================================
// 2D value noise + fbm
// ============================================================
float hash2(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float noise2(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = hash2(i);
    float b = hash2(i + vec2(1.0, 0.0));
    float c = hash2(i + vec2(0.0, 1.0));
    float d = hash2(i + vec2(1.0, 1.0));
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm2(vec2 p) {
    float v = 0.0;
    float amp = 0.5;
    for (int i = 0; i < 3; i++) {
        v += amp * noise2(p);
        p *= 2.07;
        amp *= 0.5;
    }
    return v;
}

// ============================================================
void fragment() {
    // 以 ColorRect 中心为原点，做宽高比校正，保证光晕是正圆
    vec2 rel = (UV - vec2(0.5)) * vec2(aspect, 1.0);
    float d = length(rel);
    float angle = atan(rel.y, rel.x);

    vec3 col = vec3(0.0);

    // ---- 1. 中心核心：小而亮，模拟光源本体 ----
    float core = pow(max(0.0, 1.0 - d / max(core_size, 1e-4)), core_power);
    col += core_color.rgb * core * core_intensity;

    // ---- 2. 外圈柔光：大而柔，模拟光在空气中的散射 ----
    float halo = pow(max(0.0, 1.0 - d / max(halo_size, 1e-4)), halo_power);
    col += halo_color.rgb * halo * halo_intensity;

    // ---- 3. 放射状光束：在单位圆上采样 2D 噪声，天然无缝环绕 ----
    vec2 dir = vec2(cos(angle), sin(angle));
    // 半径 = ray_count / 2π → 绕一圈恰好 ray_count 个噪声单元
    vec2 npos = dir * float(ray_count) / 6.2831853;
    npos += vec2(0.0, TIME * ray_speed);

    float n = fbm2(npos);
    n = pow(n, mix(3.5, 1.0, ray_sharpness));   // 提高对比度
    n = mix(0.5, 1.6, n);                        // 重新抬亮

    // 光束从中心向外衰减，并在最中心处淡出（那里交给核心）
    float radial = pow(max(0.0, 1.0 - d / max(ray_length, 1e-4)), ray_power);
    radial *= smoothstep(0.0, ray_inner_fade, d);

    col += ray_color.rgb * radial * n * ray_intensity;

    // ---- 全局强度 ----
    col *= global_intensity;

    COLOR = vec4(col, 1.0);
}