package we.code.demo.model.entity;

import lombok.Getter;
import lombok.Setter;

@Getter @Setter
public class Address {
    public enum State {
        QLD,
        NSW,
        ACT,
        VIC,
        TAS,
        NT,
        SA,
        WA
    }

    private String streetLine1;
    private String streetLine2;
    private String suburb;
    private State state;
    private String postCode;

    public Address(String streetLine1, String streetLine2, String suburb, State state, String postCode) {
        this.streetLine1 = streetLine1;
        this.streetLine2 = streetLine2;
        this.suburb = suburb;
        this.state = state;
        this.postCode = postCode;
    }
}
