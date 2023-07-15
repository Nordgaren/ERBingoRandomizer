namespace ERBingoRandomizer.Randomizer;

public class SeedInfo {
    public SeedInfo(string seed, string sha256Hash) {
        Seed = seed;
        SHA256Hash = sha256Hash;
    }
    public string Seed { get; }
    public string SHA256Hash { get; }
}
