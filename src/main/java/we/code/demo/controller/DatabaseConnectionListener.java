package we.code.demo.controller;

import com.zaxxer.hikari.HikariConfig;
import com.zaxxer.hikari.HikariDataSource;
import jakarta.servlet.ServletContextEvent;
import jakarta.servlet.ServletContextListener;

import java.sql.Connection;
import java.sql.SQLException;

// This is unused at the moment.
//@WebListener
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
            System.err.println("Failed to establish database connection");
        }
    }

    @Override
    public void contextDestroyed(ServletContextEvent sce) {
        if (dataSource != null) {
            dataSource.close();
            System.out.println("Database connection closed");
        }
    }
}
