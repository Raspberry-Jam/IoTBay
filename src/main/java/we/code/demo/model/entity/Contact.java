package we.code.demo.model.entity;

import lombok.Getter;
import lombok.Setter;

@Setter @Getter
public class Contact {
    private String givenName;
    private String surname;
    private String phoneNumber;
    private String emailAddress;

    public Contact(String givenName, String surname, String phoneNumber, String emailAddress) {
        this.givenName = givenName;
        this.surname = surname;
        this.phoneNumber = phoneNumber;
        this.emailAddress = emailAddress;
    }
}
