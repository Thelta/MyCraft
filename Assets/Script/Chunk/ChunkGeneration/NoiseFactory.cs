public static class NoiseFactory
{
    static ObjectPool<FastNoise> m_noisePool;
    static ObjectPool<BiomeNoises> m_biomeNoisesPool;

    static NoiseFactory()
    {
        m_noisePool = new ObjectPool<FastNoise>(() => GetFastNoiseInstance(false));
        m_biomeNoisesPool = new ObjectPool<BiomeNoises>(() => GetBiomeNoisesInstance(false));
    }

    private static FastNoise GetFastNoiseInstance(bool usePool = true)
    {
        return usePool ? m_noisePool.GetObject() : new FastNoise();
    }

    private static BiomeNoises GetBiomeNoisesInstance(bool usePool = true)
    {
        return usePool ? m_biomeNoisesPool.GetObject() : new BiomeNoises(GetTerraNoise(false), GetTreeNoise(false), GetBiomeSelectNoise(false));
    }

    public static FastNoise GetTerraNoise(bool usePool = true)
    {
        FastNoise terraNoise = GetFastNoiseInstance(usePool);
        terraNoise.SetFractalType(FastNoise.FractalType.FBM);
        terraNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

        return terraNoise;
    }

    public static FastNoise GetTreeNoise(bool usePool = true)
    {
        FastNoise treeNoise = GetFastNoiseInstance(usePool);
        treeNoise.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);
        treeNoise.SetCellularJitter(0.2f);
        treeNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
        treeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);

        return treeNoise;
    }

    public static FastNoise GetBiomeSelectNoise(bool usePool = true)
    {
        FastNoise biomeLookupNoise = new FastNoise();
        biomeLookupNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);

        FastNoise biomeNoise = GetFastNoiseInstance(usePool);
        biomeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        biomeNoise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);
        biomeNoise.SetCellularNoiseLookup(biomeLookupNoise);
        biomeNoise.SetCellularJitter(0.75f);

        return biomeNoise;
    }

    public static BiomeNoises GetBiomeNoises(bool usePool = true)
    {
        return GetBiomeNoisesInstance(usePool);
    }

    public static void PutNoiseInstance(FastNoise noise)
    {
        m_noisePool.PutObject(noise);
    }

    public static void PutBiomeNoisesInstance(BiomeNoises noises)
    {
        m_biomeNoisesPool.PutObject(noises);
    }
}

public struct BiomeNoises
{
    public BiomeNoises(FastNoise terraNoise, FastNoise treeNoise, FastNoise biomeSelectNoise)
    {
        this.terraNoise = terraNoise;
        this.treeNoise = treeNoise;
        this.biomeSelectNoise = biomeSelectNoise;
    }

    public FastNoise terraNoise { get; }
    public FastNoise treeNoise { get; }
    public FastNoise biomeSelectNoise { get; }
}
