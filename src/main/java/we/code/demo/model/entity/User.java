package we.code.demo.model.entity;

import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
public class User {
    private final String username;
    @Setter private String password;
    @Setter private Contact contact;
    @Setter private Address address;
    @Setter private String sessionToken;
    @Setter private LocalDateTime sessionExpiry;

    public User(String username, String password, Contact contact, Address address) {
        this.username = username;
        this.password = password;
        this.contact = contact;
        this.address = address;
    }
}
