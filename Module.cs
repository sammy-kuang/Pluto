public class Module {
    public Pluto pluto;

    public Module(Pluto pluto) {
        this.pluto = pluto;
    }
    public virtual void Start() {}
    public virtual void Update() {}
    public virtual void Draw() {}
    public virtual void Close() {}
}