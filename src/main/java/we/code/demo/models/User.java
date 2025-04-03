package we.code.demo.models;

import lombok.Getter;
import lombok.Setter;

public class User {
    @Getter private String username;
    @Getter @Setter private String password;

    public User(String username, String password) {
        this.username = username;
        this.password = password;
    }
}
