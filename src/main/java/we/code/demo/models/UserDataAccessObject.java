package we.code.demo.models;

import lombok.Getter;

import java.util.ArrayList;
import java.util.List;

public class UserDataAccessObject {
    @Getter private static List<User> users = new ArrayList<>();

    public static void saveUser(User user) {
        users.add(user);
    }
}
