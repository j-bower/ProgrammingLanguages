using System;
using System.IO;
using System.Collections.Generic;

struct Coords {
    public int x;
    public int y;

    public Coords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(Object obj) {
        return obj is Coords c && this == c;
    }

    public override int GetHashCode() {
        return this.x.GetHashCode() ^ this.y.GetHashCode();
    }

    public static bool operator ==(Coords a, Coords b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Coords a, Coords b) {
        return !(a == b);
    }
}

class Level {
    public int xlen;
    public int ylen;
    public Location[,] locations;
    public Exit exit {get; set;}
   
    public Level() {
        this.exit = null;
    }
}

class Location {
    public Coords coords {get; set;}
    List<Entity> entities;

    public Location() {
    }
    public Location(int x, int y) {
        this.coords = new Coords(x, y);
        this.entities = new List<Entity>();
    }

    public virtual void look() {
        Console.WriteLine("Not much to see here.");
    }

    public List<Entity> get_ents() {
        return this.entities;
    }
    public void add_ent(Entity ent) {
        this.entities.Add(ent);
    }
}
class Exit : Location {
    
    public Exit(int x, int y) {
        this.coords = new Coords(x, y);
    }

    public override void look() {
        Console.WriteLine("That looks like the gate out of this spooky place!");
    }
}

abstract class Entity {
    public virtual  void look() {}
    public abstract void interact(Player player);
}
class Key : Entity {

    public override void look() {
        Console.WriteLine("You see a key on the ground! Might need that to get out of here...");
    }
    public override void interact(Player player) {
        Console.WriteLine("You picked up a key!");
        player.set_key();
        
    }

}
class Loot : Entity {
    public int treasure {get; set;}
    public override void look() {
        if (this.treasure > 0) {
            Console.WriteLine("You see what looks like the corner of a treasure chest poking out of the ground.");
        } else {
            Console.WriteLine("A treasure chest sits already opened.");
        }

    }
    public override void interact(Player player) {
        if (this.treasure > 0) {
            Console.WriteLine($"You open the chest and find {this.treasure} coins!");
            player.treasure += this.treasure;
            this.treasure = 0;
        } else {
            Console.WriteLine("The chest is empty..."); 
        }
    }
}
class Skeleton : Entity {

    public override void look() {
        //Console.WriteLine("Not much to see here.");
    }
    public override void interact(Player player) {
        Console.WriteLine("A bony arm juts out of the ground and grabs your ankle!");
        Console.WriteLine("You've been dragged six feet under by a skeleton.");
        player.set_alive();
    }
}

class Player {
    public Coords coords { get; set; }
    public int treasure {get; set;}
    private bool key;
    private bool alive;

    public Player() {
        this.coords = new Coords(0, 0);
        this.key = false;
        this.alive = true;
        this.treasure = 0;
    }

    public bool is_at(Coords xy) {
        return this.coords == xy;
    }

    public bool is_alive() { return alive; }
    public void set_alive() {this.alive = false;}

    public bool has_key() { return key; }
    public void set_key() {this.key = true;}

    public void print_stats() {
        Console.WriteLine($"  LOCATION: {this.coords.x}, {this.coords.y}");
        Console.WriteLine($"  COINS:    {this.treasure}");
        Console.WriteLine($"  KEY:      {this.key}");
        Console.WriteLine($"  DEAD:     {!this.alive}");
    }
}

class Game {
    int    num_turns;
    Level  level;
    public Player player { get; }

    public Game() {
        this.player = new Player();
    }
    public Level get_level() {
        return this.level;
    }

    public void load(string path) {
        this.level = new Level();

        string line;
        using (StreamReader reader = new StreamReader(path)) {
            while ((line = reader.ReadLine()) != null) {
                if (line == "") { continue; }

                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length < 3) {
                    Console.WriteLine($"Bad command in level file: '{line}'");
                    Environment.Exit(1);
                }

                int x     = int.Parse(split[1]);
                int y     = int.Parse(split[2]);
                int count = 0;

                if (split.Length > 3) {
                    count = int.Parse(split[3]);
                }

                switch (split[0]) {
                    case "size":
                        // Set the level's size to x by y
                        this.level.xlen = x;
                        this.level.ylen = y;
                        this.level.locations = new Location[x,y];
                        for (int i = 0; i < x; i++) {
                            for (int j = 0; j < y; j++) {
                                this.level.locations[i,j] = new Location(i, j);
                            }
                        }
                        break;
                    case "exit":
                        // Set the level's exit location to be x, y
                        Exit ex = new Exit(x, y);
                        this.level.locations[x,y] = ex;
                        this.level.exit = ex;
                        break;
                    case "key":
                        // Add a key to location x, y
                        Key k = new Key();
                        this.level.locations[x,y].add_ent(k);
                        break;
                    case "loot":
                        // Add loot to location x, y with count coins
                        Loot l = new Loot();
                        l.treasure = count;
                        this.level.locations[x,y].add_ent(l);
                        break;
                    case "skeleton":
                        // Add a skeleton to location x, y
                        Skeleton s = new Skeleton();
                        this.level.locations[x,y].add_ent(s);
                        break;
                    default:
                        Console.WriteLine($"Bad command in level file: '{line}'");
                        Environment.Exit(1);
                        break;

                }
            }
        }
    }

    public void input(string line) {
        this.num_turns += 1;

        // Check for exhaustion?
        if (this.num_turns > 2*this.level.xlen*this.level.ylen) {
            Console.WriteLine("You have died from exhaustion.");
            this.player.set_alive();
            return;
        }

        Console.WriteLine("================================================================");

        string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (split.Length != 2) {
            Console.WriteLine($"Bad command in input: '{line}'");
            return;
        }

        Coords new_coords = this.player.coords;
        switch (split[1]) {
            case "north":
                new_coords.y += 1;
                break;
            case "south":
                new_coords.y -= 1;
                break;
            case "east":
                new_coords.x += 1;
                break;
            case "west":
                new_coords.x -= 1;
                break;
            default:
                Console.WriteLine($"Bad command in input: '{line}'");
                return;
        }

        // Are the new coords valid?
        if (new_coords.x >= this.level.xlen || new_coords.y >= this.level.ylen ||
            new_coords.x < 0 || new_coords.y < 0) {
            Console.WriteLine("A towering wall is before you. This must be the edge of the graveyard.");
            return;
        }
        List<Entity> ents;
        switch (split[0]) {
            case "go":
                this.player.coords = new_coords;
                // Need to look at the new location and interact with it.
                if (this.level.locations[new_coords.x, new_coords.y] == this.level.exit) {
                    this.level.exit.look();
                    if (this.player.has_key()) Console.WriteLine("You open the gate with your key!");
                    else {Console.WriteLine("You try to open the gate, but it's locked. Must need a key...");};
                    break;
                }
                ents = this.level.locations[new_coords.x, new_coords.y].get_ents();
                if (ents.Count > 0) {
                    if (ents.Count == 1 && ents[0].GetType() == typeof(Skeleton)) {
                        this.level.locations[new_coords.x, new_coords.y].look();
                    } else {
                        foreach (Entity ent in ents) {
                            ent.look();
                        }
                    }
                    foreach (Entity ent in ents.ToArray()) {
                        ent.interact(this.player);
                        if (ent.GetType() == typeof(Key)) {
                            ents.Remove(ent);
                        }
                    }
                } else {
                    this.level.locations[new_coords.x, new_coords.y].look();
                }
                break;
            case "look":
                // Need to look at the location.
                if (this.level.locations[new_coords.x, new_coords.y] == this.level.exit) {
                    this.level.exit.look();
                    break;
                }
                ents = this.level.locations[new_coords.x, new_coords.y].get_ents();
                if (ents.Count == 0) {
                    this.level.locations[new_coords.x, new_coords.y].look();
                }
                else {
                    if (ents.Count == 1 && ents[0].GetType() == typeof(Skeleton)) {
                        this.level.locations[new_coords.x, new_coords.y].look();
                    } else {
                        foreach (Entity ent in ents) {
                            ent.look();
                        }
                    }  
                }
                break;
            default:
                Console.WriteLine($"Bad command in input: '{line}'");
                return;
        }
    }

    bool is_over() {
        // What are the exit conditions?
        if ((this.player.coords == this.level.exit.coords) && this.player.has_key()) {
            return true;
        }
        if (!this.player.is_alive()) return true;
        return false;
    }

    void print_stats() {
        if (this.is_over() && player.is_alive()) {
            Console.WriteLine("You successfully escaped the graveyard!");
        } else {
            Console.WriteLine("You did not escape the graveyard. GAME OVER");
        }
        Console.WriteLine($"Game ended after {this.num_turns} turn(s).");
        player.print_stats();
    }

    public void exit() {
        Console.WriteLine("================================================================");
        this.print_stats();
        Environment.Exit(0);
    }

    public void exit_if_over() {
        if (this.is_over()) { this.exit(); }
    }

    public void intro() {
        Console.WriteLine("You awake in a daze to find yourself alone in the dead of night, surrounded by headstones...");
        Console.WriteLine("You must escape this graveyard.");
        Console.WriteLine("================================================================");
        // Look at the current location.
        this.level.locations[0,0].look();
        Console.Write($"{this.player.coords.x}, {this.player.coords.y}> ");
    }
}

class ETG {
    static void Main(string[] args) {
        if (args.Length != 1) {
            Console.WriteLine("ERROR: expected a single argument (the level file)");
            Environment.Exit(1);
        }

        Game game = new Game();

        game.load(args[0]);
        game.intro();

        game.exit_if_over();

        string line;

        while ((line = Console.ReadLine()) != null) {
            if (line == "") { continue; }
            game.input(line);
            game.exit_if_over();
            Console.Write($"{game.player.coords.x}, {game.player.coords.y}> ");
        }

        game.exit();
    }
}
