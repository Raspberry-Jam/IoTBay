package we.code.demo.model.entity;

import lombok.Getter;
import lombok.Setter;

@Getter
public class User {
    private final String username;
    @Setter private String password;
    @Setter Contact contact;
    @Setter Address address;

    public User(String username, String password, Contact contact, Address address) {
        this.username = username;
        this.password = password;
        this.contact = contact;
        this.address = address;
    }
}
