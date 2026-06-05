/// <summary>
/// Level item kategorileri
/// </summary>
public enum ItemType
{
    /// <summary>
    /// Topun hareketini değiştiren item'ler (rüzgar, trambolin, mıknatıs vb.)
    /// </summary>
    MovementModifier,
    
    /// <summary>
    /// Engeller (dönen çark, kapılar, lazer vb.)
    /// </summary>
    Obstacle,
    
    /// <summary>
    /// Özel efektler (portal, hız boost, slow motion vb.)
    /// </summary>
    SpecialEffect,
    
    /// <summary>
    /// Hedef mekanikleri (buton, yıldız, sıralı hedefler vb.)
    /// </summary>
    TargetMechanic
}
