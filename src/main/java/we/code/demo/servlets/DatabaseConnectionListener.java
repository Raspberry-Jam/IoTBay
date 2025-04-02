package we.code.demo.servlets;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;
import jakarta.servlet.ServletContextEvent;
import jakarta.servlet.ServletContextListener;
import jakarta.servlet.annotation.WebListener;

import java.sql.Connection;
import java.sql.SQLException;

@WebListener
public class DatabaseConnectionListener implements ServletContextListener {
    private HikariDataSource dataSource;

    @Override
    public void contextInitialized(ServletContextEvent sce) {
        HikariConfig config = new HikariConfig("/hikari.properties");
        dataSource = new HikariDataSource(config);
        sce.getServletContext().setAttribute("dataSource", dataSource);
        try(Connection connection = dataSource.getConnection()) {
            System.out.println("Database connection established");
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void contextDestroyed(ServletContextEvent sce) {
        if (dataSource != null) {
            dataSource.close();
        }
        System.out.println("Database connection closed");
    }
}
