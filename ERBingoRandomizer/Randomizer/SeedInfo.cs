namespace ERBingoRandomizer.Randomizer; 

public class SeedInfo {
    public string Seed { get; }
    public string SHA256Hash { get; }
    public SeedInfo(string seed, string sha256Hash) {
        Seed = seed;
        SHA256Hash = sha256Hash;
    }
}
