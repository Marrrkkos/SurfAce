void SingleWave_float(float3 Position, float4 DataA, float4 DataB, float4 DataC, float Time, out float3 Offset)
{
    Offset = float3(0, 0, 0);
    
    float wavelength = DataB.y;

    if(wavelength < 1){
        return;
    }

    float2 origin = DataA.xy;      // x und y sind Origin
    float2 direction = DataA.zw;   // z und w sind Direction

    float speed = DataB.x;
    float steepness = DataB.z;
    float startTime = DataB.w;

    float heightScale = DataC.x;

    // 1. Sicherheit & Konstanten
    float2 d = normalize(direction); // Input ist Vektor!

    float k = 6.28318 / wavelength; 
    float c = sqrt(9.8 / k) * speed;

    // 2. Zeit
    float timeSinceStart = Time - startTime;

    if (timeSinceStart < 0)
    {
        Offset = float3(0, 0, 0);
        return;
    }

    // 3. Distanz der Welle vom Ursprung
    float travelDist = c * timeSinceStart;

    // Projektion der aktuellen Pixel-Position auf den Richtungsvektor der Welle
    float distAlongDir = dot(d, Position.xz - origin);
    float distFromCrest = distAlongDir - travelDist;

    // 4. Gaußsche Maske
    float sigma = wavelength * 0.5;

    // Optimierung: Wenn Pixel zu weit von der Wellenfront weg ist -> 0
    if (abs(distFromCrest) > wavelength * 4.0) 
    {
        Offset = float3(0, 0, 0);
        return;
    }

    float mask = exp(-(distFromCrest * distFromCrest) / (2.0 * sigma * sigma));

    // 5. Gerstner mit Maske
    float f = k * distAlongDir - c * Time;
    float a = (steepness / k) * mask;

    a *= heightScale;
    float cosF = cos(f);
    float sinF = sin(f);

    Offset.x = d.x * (a * cosF);
    Offset.y = a * sinF;
    Offset.z = d.y * (a * cosF);
}