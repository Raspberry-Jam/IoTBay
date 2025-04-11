package we.code.demo.model.dao;

import lombok.Getter;
import we.code.demo.model.entity.User;

import java.util.ArrayList;
import java.util.List;

public class UserDataAccessObject {
    @Getter private static List<User> users = new ArrayList<>();

    public static void addUser(User user) {
        users.add(user);
    }
}
