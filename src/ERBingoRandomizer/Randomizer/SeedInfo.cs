namespace ERBingoRandomizer.Randomizer;

public class SeedInfo {
    public SeedInfo(string seed, string sha256Hash) {
        Seed = seed;
        Sha256Hash = sha256Hash;
    }
    public string Seed { get; }
    public string Sha256Hash { get; }
}
